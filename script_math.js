#!/usr/bin/env node

var amqp = require('amqplib/callback_api');
amqp.connect('amqp://localhost', function(err, conn) {
  conn.createChannel(function(err, ch) {
    var q = 'Queue_math';
    var a = $('. a ').val();
    var b = $('. b ').val();
    var pair = {
       first: a,
       second: b
    };
    var message = JSON.stringify(pair);
    ch.assertQueue(q, {durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null});
    ch.sendToQueue(q, new Buffer(message));
    console.log(" [x] Sent up queery");
  });
  setTimeout(function() { conn.close(); process.exit(0) }, 5000);
});