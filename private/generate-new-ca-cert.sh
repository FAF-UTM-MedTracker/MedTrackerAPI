openssl req \
    -new \
    -x509 \
    -config ca-cert.cfg \
    -keyout ca-private-key.pem \
    -out ca-cert.pem \
    -nodes