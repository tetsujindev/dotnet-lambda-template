### 作成する順番
1. remote-state-storage
1. vpc
1. rds
1. lambda
1. api-gateway

### 削除する順番
1. api-gateway
1. lambda
1. rds
1. vpc
1. remote-state-storage

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
