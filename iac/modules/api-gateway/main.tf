terraform {
  required_version = ">= 1.0.0, < 2.0.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.83.1"
    }
  }
}

resource "aws_api_gateway_rest_api" "root" {
  endpoint_configuration {
    types = ["REGIONAL"]
  }
  name = var.api_name
}

resource "aws_api_gateway_resource" "books" {
  parent_id   = aws_api_gateway_rest_api.root.root_resource_id
  path_part   = "books"
  rest_api_id = aws_api_gateway_rest_api.root.id
}

resource "aws_api_gateway_method" "get" {
  api_key_required = "false"
  authorization    = "NONE"
  http_method      = "GET"
  resource_id      = aws_api_gateway_resource.books.id
  rest_api_id      = aws_api_gateway_rest_api.root.id
}

resource "aws_api_gateway_integration" "get_books" {
  http_method             = "GET"
  integration_http_method = "POST"
  resource_id             = aws_api_gateway_resource.books.id
  rest_api_id             = aws_api_gateway_rest_api.root.id
  type                    = "AWS_PROXY"
  uri                     = var.get_books_invoke_arn
}

resource "aws_api_gateway_method_response" "get_books_200" {
  http_method = "GET"
  resource_id = aws_api_gateway_resource.books.id

  response_models = {
    "application/json" = "Empty"
  }

  rest_api_id = aws_api_gateway_rest_api.root.id
  status_code = "200"
  depends_on = [ aws_api_gateway_integration.get_books ]
}

resource "aws_api_gateway_deployment" "default" {
  rest_api_id = aws_api_gateway_rest_api.root.id

  triggers = {
    redeployment = sha1(jsonencode([
      aws_api_gateway_rest_api.root.body,
      aws_api_gateway_resource.books,
      aws_api_gateway_method.get,
      aws_api_gateway_integration.get_books,
    ]))
  }

  lifecycle {
    create_before_destroy = true
  }

  depends_on = [ aws_api_gateway_integration.get_books ]
}

resource "aws_api_gateway_stage" "default" {
  deployment_id         = aws_api_gateway_deployment.default.id
  rest_api_id           = aws_api_gateway_rest_api.root.id
  stage_name            = "default"
}

resource "aws_lambda_permission" "api_permission" {
  action        = "lambda:InvokeFunction"
  function_name = var.get_books_function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${aws_api_gateway_rest_api.root.execution_arn}/*/*/*"
}
