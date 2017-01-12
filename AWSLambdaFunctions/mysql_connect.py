import sys
import logging
import rds_config
import pymysql
import json

#rds settings
rds_host  = "ssdb.cbmc3ks2dvaj.us-east-1.rds.amazonaws.com"
name = rds_config.db_username
password = rds_config.db_password
db_name = rds_config.db_name
port = 3306

def connect():
    try:
        conn = pymysql.connect(host=rds_host, user=name, passwd=password, db=db_name, port=3306, connect_timeout=5, cursorclass=pymysql.cursors.DictCursor)
        return conn
    except:
        print("ERROR: Unexpected error: Could not connect to MySql instance.")
        sys.exit()
