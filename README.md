# TCP Bancho "Proxy" for <a href="https://github.com/Zordon1337/osu2007srv">osu2007srv</a>

## Warning! this is very simple and not really stable, just for now its proof of concept. It works like it should but, likes to crash sometimes, slow time response, client reconnects automatically after ~~10-20 secs
if you want to use clients using tcp to communicate you need to use this Proxy, to fetch data from http backend and respond with them by sockets

this example gonna be good explaination
- client joins writes info about user and Asks for login reply packet
- server makes request to instance with path /web/osu-login.php
- if response is 1(everything is fine, can connect) then server sends LoginReply packet with random id. If the response is 0(something is not matching) then server responds with id -1(bad pass or username)
- then server makes request to path /web/osu-statoth.php to retreive stats such as Total score, Rank and accuracy then respond to client With UserStats Packet containing that data, and you may ask what happens
  to value not introduced in http backend such as Player count, Ranked score and Status, Well they are basically filled with zero's or default values because we can't do much abt it

How to setup?
1. Clone repo
2. change backend urls in file
3. Build using dotnet or visual studio
4. run, now force client to connect to this endpoint
   
