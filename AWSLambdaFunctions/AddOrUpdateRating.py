import sys
import logging
import rds_config
import mysql_connect
import pymysql
import json

def handler(event, context):
    """
    This function fetches content from mysql RDS instance
    """
    if(not validate(event)):
        return { "Message" : "Invalid data" }
        
    conn = mysql_connect.connect()
    
    try:
        with conn.cursor() as cur:
            
            rating = int(event["Stars"])
            if(rating < 1 or rating > 5):
                returnVal = { "Message" : "Invalid rating" }
                sys.exit()
            
            # Check if rating already exists
            sql = "SELECT RatingID FROM Rating WHERE LevelID=%s AND RaterID=%s"
            cur.execute(sql, (event["LevelID"], event["PlayerID"]))
            result = cur.fetchone() #Returns None or ID
            
            if(result == None): #Add the rating
                sql = "INSERT INTO Rating (LevelID, RaterID, Stars) VALUES (%s, %s, %s)"
                cur.execute(sql, (event["LevelID"], event["PlayerID"], rating))
            else:
                sql = "UPDATE Rating SET Stars=%s WHERE RatingID=%s"
                cur.execute(sql, (rating, result["RatingID"]))
            
            conn.commit()
            returnVal = { "Message" : "Success" }
            
    except Exception, err:
        returnVal = { "Message" : str(err) }
    
    finally:
        conn.close()
        return returnVal;
        
def validate(event):
    return (event.has_key("PlayerID") and event.has_key("LevelID") and event.has_key("Stars"))
    
'''
def main():
    print(handler({
    "PlayerID": "TestID",
    "LevelID":"5",
    "Stars":"5"
    }, None))
    
if True:
    main()
 '''