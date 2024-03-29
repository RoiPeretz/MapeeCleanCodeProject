﻿using System.Text;
using MessageBroker.Core.Interfaces;
using MessageBroker.Infrastructure.RabbitMq.CreateConnectionWorkFlow;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace MessageBroker.Infrastructure.RabbitMq;

internal class RabbitMqPublisher : IPublisher, IDisposable
{
    private readonly IModel _channel;
    private readonly ILogger _logger;

    public RabbitMqPublisher(IRabbitMqCreateConnectionWorkFlow rabbitMqCreateConnectionWorkFlow,
        ILogger<RabbitMqPublisher> logger)
    {
        _channel = rabbitMqCreateConnectionWorkFlow.Init();
        _logger = logger;
    }

    public void Dispose()
    {
        _channel.Dispose();
    }

    public void Publish(string topic, string message)
    {
        try
        {
            _channel.ExchangeDeclare(topic, ExchangeType.Fanout);

            _channel.QueueDeclare(topic,
                false,
                false,
                false,
                null);

            var messageInBytes = Encoding.UTF8.GetBytes(message);
            Thread.Sleep(300);
            _channel.BasicPublish("",
                topic,
                null,
                messageInBytes);
            _logger.LogInformation(" [x] Sent {0}", message);
        }
        catch (Exception ex)
        {
            //TODO - set result when we add return type
            _logger.LogError(ex.Message);
        }
    }
}