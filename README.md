
# Imagize.NET

![GitHub Workflow Status](https://img.shields.io/github/workflow/status/captivereality/imagize/.NET?style=for-the-badge) ![Docker Image Version (tag latest semver)](https://img.shields.io/docker/v/captivereality/imagize/latest?style=for-the-badge)

Simple fast and modern HTTP microservice written in C# for image processing.  Initially this library will allow for resizing images but the intention is that it will be extended over time to do far more such as cropping, watermarking etc.

The general idea is that you can use it as a proxy to manipulate images on the fly.

The service has built in protection to ensure that it can only be used to manipulate images from predetermined sources.

The library will be able to use any image processing library as long as the library is cross platform. Initially we're only using SkiaSharp but the intention is to support the best features across multiple libraries.

The other primary objective is to make it as accessible as possible such that anyone can run and consume the service and in order to do this we need to ensure it will always build and run in Linux containers.

## Contents

  * [Requirements](#requirements)
  * [Cloning the Repo (for Development)](#cloning-the-repo--for-development-)
  * [Building](#building)
  * [Usage](#usage)
  * [Environment Variables](#environment-variables)
  * [Docker](#docker)
  * [Docker Compose](#docker-compose)
  * [Roadmap](#roadmap)
  * [Change Log](#change-log)
  * [Author/s](#author-s)
  * [Copyright](#copyright)
  * [Licence](#licence)

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

## Environment Variables

Multiple env vars should be separated with either pipes `|`, tilde's `~` or commas `,` eg...  var1|var2

|Env Var       |Description              |Example|
|-----------|-------------------------|------|
|ASPNETCORE_ENVIRONMENT|Development Environment|`Development` or `Production` |
|IMAGIZE_ALLOWED_FILETYPES|Supported Filetypes| `jpg~jpeg~png~gif~heic~heif` |
|IMAGIZE_ALLOWED_ORIGINS|Allowed Origins | `http://www.mysite.com~https://s3.eu-west-2.amazonaws.com/my-s3` |

Note: On Windows you may need to include double slashes.. eg for every slash add a further slash.


## Docker

See Dockerfile for image details

Fetch the image from docker

	docker pull captivereality/imagize:latest

Start the container on port 9000 with an interactive shell and remove it once it stops

	docker run -p 0.0.0.0:9000:80/tcp -e 'ASPNETCORE_ENVIRONMENT=Development' -e 'ASPNETCORE_URLS=http://+:80' -e 'IMAGIZE_ALLOWED_FILETYPES=jpg~jpeg~png~gif~heic' -e 'IMAGIZE_ALLOWED_ORIGINS=https://www.website.com~https://website.com~https://s3.eu-west-2.amazonaws.com/your-s3' --name imagize captivereality/imagize:latest -it -rm

Check it's running...

	docker ps

Test it..

	http://127.0.0.1:9000/swagger/index.html

## Docker Compose


An example `docker-compose.yml`

```yaml
version: '3'
services:
  imagize:
    image: 'captivereality/imagize:latest'
    container_name: imagize
    restart: unless-stopped
    mem_limit: 300m
    ports:
      - '9000:80'
    environment:
      - IMAGIZE_ALLOWED_FILETYPES=jpg|jpeg|png|gif|heic
      - IMAGIZE_ALLOWED_ORIGINS=https://www.website.com|https://website.com|https://s3.eu-west-2.amazonaws.com/your-s3
    networks:
      - hosting

networks:
  hosting:
    name: hosting
```

Bring it up with...

	docker-compose up -d


## Roadmap

- [x] SkiaSharp Provider
- [x] Image Resize
- [x] Server Health
- [x] Image Exif (full)
- [ ] Crop Image
- [ ] Watermark Image
- [ ] Zoom Image
- [ ] Auto Rotate Image
- [ ] Rotate Image
- [ ] Blur Image


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