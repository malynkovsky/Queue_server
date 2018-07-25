#!/usr/bin/env node

var amqp = require('amqplib/callback_api');
amqp.connect('amqp://localhost', function(err, conn) {
  conn.createChannel(function(err, ch) {
    var q = 'Queue_up';
    var message = $('. innerglass ').attr('height');
    ch.assertQueue(q, {durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null});
    ch.sendToQueue(q, new Buffer(message));
    console.log(" [x] Sent up queery");
  });
  setTimeout(function() { conn.close(); process.exit(0) }, 5000);
});
//$(".innerglass1").animate({height: "100px"}, 3500);
//$(".innerglass1").animate({height: "0px"}, 3500);