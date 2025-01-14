#!/bin/bash

set -euo pipefail

for f in example.cossacklabs.com.crt server.crt server.key; do
    cp /tmp.ssl/${f} "${PGDATA}/"
    chown postgres:postgres "${PGDATA}/${f}"
    chmod 0600 "${PGDATA}/${f}"
done

set_pg_option() {
    sed -i "s/^#*${1}\\s*=.*/${1} = ${2}/g" "$PGDATA/postgresql.conf"
}

set_pg_option "listen_addresses" "'*'"
set_pg_option "ssl" "on"
set_pg_option "ssl_ca_file" "'example.cossacklabs.com.crt'"
set_pg_option "ssl_cert_file" "'server.crt'"
set_pg_option "ssl_key_file" "'server.key'"

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
	create table customer(name text, email text, debitcard bytea);