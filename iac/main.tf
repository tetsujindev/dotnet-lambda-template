provider "aws" {
  region = "ap-northeast-1"  
}

resource "null_resource" "lambda_build" {
  triggers = {
    always_run = "${timestamp()}"
  }

  provisioner "local-exec" {
    command = "dotnet lambda package --project-location ../src/Lambda/HelloLambda/src/HelloLambda/ --output-package ../src/Lambda/HelloLambda/src/HelloLambda/bin/HelloLambda.zip"
  }
}

resource "aws_lambda_function" "hello_function" {
  filename         = "../src/Lambda/HelloLambda/src/HelloLambda/bin/HelloLambda.zip"
  function_name    = "HelloLambda"
  role             = "arn:aws:iam::194722443726:role/lambda-administrator-role"
  handler          = "HelloLambda::HelloLambda.Function::FunctionHandler"
  runtime          = "dotnet8"
  depends_on = [ null_resource.lambda_build ]
}
