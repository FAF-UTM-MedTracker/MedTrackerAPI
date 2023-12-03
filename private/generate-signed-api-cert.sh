# Create private key
openssl genrsa -out api-private-key.pem

# Create certificate signing request
openssl req \
    -new \
    -nodes \
    -config api-cert.cfg \
    -extensions v3_req \
    -key api-private-key.pem \
    -out api-certificate-request.csr \

# Sign the request with the CA certificate
openssl x509 \
    -req \
    -in api-certificate-request.csr \
    -CA ca-cert.pem \
    -CAkey ca-private-key.pem \
    -CAcreateserial \
    -out api-cert.pem \
    -days 365 \
    -sha256 \
    -extfile api-cert.cfg \
    -extensions v3_req

rm api-certificate-request.csr

