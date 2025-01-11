resource "aws_api_gateway_rest_api" "root" {
  endpoint_configuration {
    types = ["REGIONAL"]
  }
  name = "root"
}

resource "aws_api_gateway_resource" "students" {
  parent_id   = aws_api_gateway_rest_api.root.root_resource_id
  path_part   = "students"
  rest_api_id = aws_api_gateway_rest_api.root.id
}

resource "aws_api_gateway_method" "get" {
  api_key_required = "false"
  authorization    = "NONE"
  http_method      = "GET"
  resource_id      = aws_api_gateway_resource.students.id
  rest_api_id      = aws_api_gateway_rest_api.root.id
}

resource "aws_api_gateway_integration" "get_all_students_async" {
  http_method             = "GET"
  integration_http_method = "POST"
  resource_id             = aws_api_gateway_resource.students.id
  rest_api_id             = aws_api_gateway_rest_api.root.id
  type                    = "AWS"
  uri                     = aws_lambda_function.student_function.invoke_arn
}

resource "aws_api_gateway_integration_response" "get_all_students_async_200" {
  http_method = "GET"
  resource_id = aws_api_gateway_resource.students.id
  rest_api_id = aws_api_gateway_rest_api.root.id
  status_code = "200"
  depends_on = [ aws_api_gateway_integration.get_all_students_async ]
}

resource "aws_api_gateway_method_response" "get_all_students_async_200" {
  http_method = "GET"
  resource_id = aws_api_gateway_resource.students.id

  response_models = {
    "application/json" = "Empty"
  }

  rest_api_id = aws_api_gateway_rest_api.root.id
  status_code = "200"
  depends_on = [ aws_api_gateway_integration.get_all_students_async ]
}

resource "aws_api_gateway_stage" "default" {
  deployment_id         = aws_api_gateway_deployment.default.id
  rest_api_id           = aws_api_gateway_rest_api.root.id
  stage_name            = "default"
}

resource "aws_api_gateway_deployment" "default" {
  rest_api_id = aws_api_gateway_rest_api.root.id

  triggers = {
    redeployment = sha1(jsonencode([
      aws_api_gateway_rest_api.root.body,
      //aws_api_gateway_resource.students,
      //aws_api_gateway_method.get,
      //aws_api_gateway_integration.get_all_students_async,
    ]))
  }

  lifecycle {
    create_before_destroy = true
  }

  depends_on = [ aws_api_gateway_integration.get_all_students_async ]
}

resource "aws_lambda_permission" "api_permission" {
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.student_function.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${aws_api_gateway_rest_api.root.execution_arn}/*/*/*"
}
