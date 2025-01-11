provider "aws" {
  region = "ap-northeast-1"
}

resource "null_resource" "lambda_build" {
  triggers = {
    md5 = "${md5(file("../src/Lambda/StudentLambda/src/StudentLambda/Function.cs"))}"
  }

  provisioner "local-exec" {
    command = "dotnet lambda package --project-location ../src/Lambda/StudentLambda/src/StudentLambda/ --output-package ../src/Lambda/StudentLambda/src/StudentLambda/bin/StudentLambda.zip"
  }
}

data "local_file" "StudentLambda" {
  filename   = "../src/Lambda/StudentLambda/src/StudentLambda/bin/StudentLambda.zip"
  depends_on = [null_resource.lambda_build]
}

resource "aws_lambda_function" "student_function" {
  filename         = data.local_file.StudentLambda.filename
  source_code_hash = data.local_file.StudentLambda.content_base64sha256
  function_name    = "StudentLambda"
  role             = "arn:aws:iam::194722443726:role/lambda-administrator-role"
  handler          = "StudentLambda::StudentLambda.Function::FunctionHandler"
  runtime          = "dotnet8"
  depends_on       = [null_resource.lambda_build]
}
