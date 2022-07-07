
# Imagize.NET

![GitHub Workflow Status](https://img.shields.io/github/workflow/status/captivereality/imagize/.NET?style=for-the-badge) ![Docker Image Version (tag latest semver)](https://img.shields.io/docker/v/captivereality/imagize/latest?style=for-the-badge)
Simple fast and modern HTTP microservice written in C# for image processing.  Initially this library will allow for resizing images but the intention is that it will be extended over time to do far more such as cropping, watermarking etc.

The general idea is that you can use it as a proxy to manipulate images on the fly.

The service has built in protection to ensure that it can only be used to manipulate images from predetermined sources.

The library will be able to use any image processing library as long as the library is cross platform. Initially we're only using SkiaSharp but the intention is to support the best features across multiple libraries.

The other primary objective is to make it as accessible as possible such that anyone can run and consume the service and in order to do this we need to ensure it will always build and run in Linux containers.

## Requirements

- Visual Studio Code, Visual Studio 2022 (or Rider)
- The package has a number of dependencies (see Nuget for a list)

## Cloning the Repo (for Development)

	git clone https://<USERNAME>@github.com/captivereality/imagize.git

## Building

	dotnet restore
	dotnet build

## Usage

todo

## Docker

See Dockerfile for image details

Fetch the image from docker

	docker pull captivereality/imagize:latest

Start the container on port 9000 with an interactive shell and remove it once it stops

	docker run -p 0.0.0.0:9000:9000/tcp -e ASPNETCORE_URLS=http://+:9000  --name imagize captivereality/imagize:latest -it -rm

Check it's running...

	docker ps



## Change Log

|Date       |Version|Description              |
|-----------|-------|-------------------------|
|2022-06-26|v0.0.1|Initial Version|

## Author/s

|Name|Contact|
|-----------|-------|
|Mark Castle|https://github.com/captivereality|

## Copyright
© 2022 Captive Reality Ltd.  All Rights Reserved.

## Licence
MIT Licence