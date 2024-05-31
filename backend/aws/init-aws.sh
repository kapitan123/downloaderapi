#!/bin/bash

echo "Init buckets"
awslocal s3 mb s3://documents && awslocal s3 mb s3://previews