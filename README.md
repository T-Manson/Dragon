# Dragon

基于 .Net Core 的后端基础框架

# 项目介绍

基于 .Net Core 的一套后端通用基础框架，用于快速搭建WebApi项目。集成Redis、Dapper、RabbitMQ、AutoMapper、Newtonsoft.Json。加入了常用工具类避免造轮子的工作。

# 技术选型

|技术|简介|官网|
|-----|-----|-----|
|.Net Core|开源跨平台的技术框架|https://dotnet.microsoft.com|
|StackExchange.Redis|Redis中间件|https://github.com/StackExchange/StackExchange.Redis|
|Dapper|ORM框架|https://github.com/StackExchange/Dapper|
|RabbitMQ.Client|RabbitMQ中间件|https://github.com/rabbitmq/rabbitmq-dotnet-client|
|AutoMapper|对象映射中间件|https://github.com/AutoMapper/AutoMapper|
|Newtonsoft.Json|Json中间件|https://github.com/JamesNK/Newtonsoft.Json|

# 功能说明

## 中文转拼音功能
Inspired by PinYinConverter.<br />
Microsoft.International.Converters.PinYinConverter in Microsoft Visual Studio International Pack 1.0 SR1.

## MemoryCache

注入接口类型：

ICacheManager

**特别说明**

默认实现消息通知起到local缓存更新后分布式应用间的数据同步功能。未启用MessageBus功能时，该功能不会启用。

当MessageBus使用RabbitMQ时，需要配置一个名为**cache.sync**的channel

## Redis

注入接口类型：

ICacheManager

## HybridCache

注入接口类型：

ICacheManager

IMemoryCacheManager

IRedisCacheManager

## RabbitMQ

注入接口类型：

IMessageBus

## RedisBus

注入接口类型：

IMessageBus

# 配置

[完整示例](Samples/Dragon.Samples.WepApi/appsettings.json)

## 应用名称

``` text
"AppName": ""
```

## 数据库

``` json
"Data": {
    // 默认使用的连接配置键名
    "DefaultConnectionName": "default",
    	// 连接串集合
	"ConnectionStrings": {
        // 默认（读串）
	"default": {
        	// 连接串
		"ConnectionString": "server=127.0.0.1;Database=mytest;UID=root;PWD=root;SslMode=None;",
            	// 数据库类型（默认MySql）
		"DatabaseProviderType": "MySql|SqlServer"
	}
        //,
        // 写串
        //"write": {
        //    "ConnectionString": "server=;Database=;UID=;PWD=;Charset=utf8;SslMode=None;",
        //    "DatabaseProviderType": "SqlServer"
        //}
	},
    // Dapper 配置
    "Dapper": {
        // 数据库映射策略（默认Underline）
	"DbIdentifierMappingStrategy": "Underline|PascalCase",
        // 大小写规则（默认LowerCase）
	"CapitalizationRule": "LowerCase|UpperCase|Original"
    }
}
```

## 缓存

``` json
"Cache": {
    	// Redis配置
	"Redis": {
        // IP，必须
        "Host": "127.0.0.1",
        // 端口，必须
        "Port": 6379,
        // 密码，必须
        "Password": "123456",
        // Key的区域区分系统，必须
        "Region": "应用名或其他能够隔离缓存的值",
        // DB下标，不配置则默认0
	"Db": 0
	}
}
```

## 消息

### RedisBus

与Cache节点下Redis共用配置。[缓存配置](#缓存)

### RabbitMQ

``` json
"RabbitMQ": {
    	// RabbitMQ默认配置
	"Default": {
        // Uri，必须
        "Uri": "127.0.0.1",
        // 用户名，必须
        "Username": "username",
        // 密码，必须
        "Password": "123456",
        // Exchange，必须
        "Exchange": "test"
	}
}
```






