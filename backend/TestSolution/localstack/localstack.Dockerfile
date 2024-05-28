FROM localstack/localstack

COPY --chown=localstack ./init.sh /etc/localstack/init/ready.d/init.sh

RUN chmod u+x /etc/localstack/init/ready.d/init.sh