#!/usr/bin/env node
var timerId = setInterval(function() {
  var amqp = require('amqplib/callback_api');
  var message = $('. innerglass ').attr('height');
  if (message == 0) { conn.close(); process.exit(0) };
  amqp.connect('amqp://localhost', function(err, conn) {
  conn.createChannel(function(err, ch) {
    var q = 'Queue_down';
    
    ch.assertQueue(q, {durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null});
    ch.sendToQueue(q, new Buffer(message));
    console.log(" [x] Sent up queery");
  }); 
});
}, 2000);
