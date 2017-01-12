import sys
import logging
import rds_config
import mysql_connect
import pymysql
import json

"""
Sets the player's name. If a name is already associated with this ID,
update the name instead.
"""

def handler(event, context):
    """
    This function fetches content from mysql RDS instance
    """
    if(not validate(event)):        
        return {"Messages" : [{"Message" : "Invalid data" }]}
        sys.exit()
    
    conn = mysql_connect.connect()


    try:
        with conn.cursor() as cur:

            if(event.has_key("Discover")):
                sql = ("SELECT LevelID, LevelName, PlayerName, PlaythroughCount " 
                    "FROM Level "
                    "LEFT JOIN Player ON Level.PlayerID=Player.PlayerID "
                    "WHERE Level.PlaythroughCount<'10' ORDER BY RAND() LIMIT 1"
                    )
            
                cur.execute(sql)
                
                result = cur.fetchall()
                returnVal =  {"Messages" : [{"Message" : "Success"}],"Levels" : result}
            else:
        
                count = int(event["Count"])
                count = min(count, 50)
                
                orderBy = ""
                
                if(event["SortBy"] == "AgeAsc"):
                    orderBy = "ORDER BY UploadTime DESC" # This is not an error
                
                elif(event["SortBy"] == "AgeDesc"):
                    orderBy = "ORDER BY UploadTime ASC"
                
                elif(event["SortBy"] == "Popularity"):
                    orderBy = "ORDER BY PlaythroughCount DESC"
                
                elif(event["SortBy"] == "Challenging"):
                    orderBy = ""
                
                elif(event["SortBy"] == "Creative"):
                    orderBy = ""
                
                elif(event["SortBy"] == "Fun"):
                    orderBy = ""
                
                #if(event.has_key("MaxAge")):
                
            
                sql = ("SELECT LevelID, LevelName, PlayerName, PlaythroughCount " 
                        "FROM Level "
                        "LEFT JOIN Player ON Level.PlayerID=Player.PlayerID"
                        " " + orderBy + " LIMIT %s") 
                    
                cur.execute(sql, (count,))
                result = cur.fetchall()
                
                returnVal =  {"Messages" : [{"Message" : "Success"}],"Levels" : result}
            
            
    except Exception, err:
        returnVal =  {"Messages" : [{"Message" : "Exception: " + str(err) }]}
        sys.exit()
        
    finally:
        conn.close()
        return returnVal
    
def validate(event):
    return ((event.has_key("Count") and
    event.has_key("SortBy")) or event.has_key("Discover")) # AgeAsc, AgeDesc, Popularity, Any Tag