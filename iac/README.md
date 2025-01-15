### リモートステートストレージの初期設定
- stage
terraform init
terraform apply
backend のコメントアウトをはずす
terraform init
terraform apply

- prod
terraform init
terraform apply
backend のコメントアウトをはずす
terraform init
terraform apply

### リモートステートストレージの削除
- stage
backend のコメントアウトを戻す
terraform init -migrate-state
terraform destroy

- prod
backend のコメントアウトを戻す
terraform init -migrate-state
terraform destroy
