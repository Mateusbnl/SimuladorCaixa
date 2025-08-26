#!/bin/bash
MAX_RETRIES=3
WAIT_SECONDS=5
import_collection() {
	local collection=$1
	local file=$2
	local retries=2
	while [ $retries -lt $MAX_RETRIES ]; do
		if [ -f "$file" ]; then
			echo "Importando colecao $collection..."
			mongoimport --db simuladorcaixa --collection "$collection" --file "$file"
			return
		else
			echo "Arquivo $file nao encontrado. Tentando novamente em $WAIT_SECONDS segundos... ($((retries+1))/$MAX_RETRIES)"
			sleep $WAIT_SECONDS
			((retries++))
		fi
	done
	echo "Falha ao encontrar o arquivo $file apos $MAX_RETRIES tentativas."
}
import_collection "Proposta" "/docker-entrypoint-initdb.d/Proposta.schema.json"
import_collection "Telemetria" "/docker-entrypoint-initdb.d/Telemetria.schema.json"