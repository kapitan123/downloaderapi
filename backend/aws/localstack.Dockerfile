#https://github.com/localstack/localstack/issues/7596
FROM localstack/localstack

COPY --chown=localstack ./init-aws.sh /etc/localstack/init/ready.d/init-aws.sh

#https://docs.localstack.cloud/references/init-hooks/
RUN chmod u+x /etc/localstack/init/ready.d/init-aws.sh