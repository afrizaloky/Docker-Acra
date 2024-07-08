FROM ubuntu:20.04


ENV DEBIAN_FRONTEND=noninteractive

RUN DEBIAN_FRONTEND=noninteractive apt -y update && apt install -y git libssl-dev make build-essential wget ssh nano mysql-client protobuf-compiler
RUN mkdir /var/run/sshd
RUN echo 'root:root' | chpasswd
RUN sed -i 's/#PermitRootLogin prohibit-password/PermitRootLogin yes/' /etc/ssh/sshd_config


ENV PATH=$PATH:/root/go/bin:/go/bin
ENV ACRA_MASTER_KEY=N0MyZEVCRGY1cHA4ODRSVHp3c3BNeGtDekxaV3BhTHI=

WORKDIR /
COPY ./files/acra-server.crt /go/
COPY ./files/acra-client.crt /go/
COPY ./files/acra-server.key /go/
COPY ./files/example.cossacklabs.com.crt /go/
COPY ./files/encryption_config.yml /go/

RUN git clone https://github.com/cossacklabs/themis.git
RUN cd themis \
    && make \
    && make install

WORKDIR /
RUN wget https://go.dev/dl/go1.21.0.linux-amd64.tar.gz
RUN tar xzvf go1.21.0.linux-amd64.tar.gz
RUN chmod 777 /go \
  && export PATH=$PATH:/go/bin \
  && go install github.com/cossacklabs/acra/cmd/acra-keymaker@0.95.0
  
WORKDIR /go/src/github.com/cossacklabs
RUN git clone https://github.com/cossacklabs/acra.git

WORKDIR /go/src/github.com/cossacklabs/acra
RUN /root/go/bin/acra-keymaker --keystore=v1 \
  --client_id="" \
  --tls_cert=/go/acra-client.crt \
  --generate_acrawriter_keys \
  --generate_symmetric_storage_key \
  --generate_hmac_key \
  --generate_log_key

EXPOSE 9090 9393
# CMD tail -f
CMD go run cmd/acra-server/acra-server.go \
  --db_host=postgresql \
  --keys_dir=.acrakeys \
  --tls_auth=4 \
  --tls_identifier_extractor_type=distinguished_name \
  --tls_ca=/go/example.cossacklabs.com.crt \
  --tls_cert=/go/acra-server.crt \
  --tls_key=/go/acra-server.key \
  --tls_database_sni=postgresql \
  --tls_client_id_from_cert=true \
  --encryptor_config_file=/go/encryption_config.yml
