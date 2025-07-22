# 🖼️ Imagize.NET

![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/captivereality/imagize/dotnet.yml?branch=master&style=for-the-badge) ![Docker Image Version (tag latest semver)](https://img.shields.io/docker/v/captivereality/imagize/latest?style=for-the-badge)

> 🚀 **Fast, modern, and secure HTTP microservice for on-the-fly image processing**

Imagize.NET is a high-performance ASP.NET Core Web API that provides real-time image manipulation capabilities. Built with a modular architecture and security-first approach, it's designed to be your go-to solution for image processing in microservice environments.

## ✨ Features

- 🔄 **Image Resizing** - Intelligent resizing with aspect ratio preservation
- ✂️ **Image Cropping** - Precise cropping with customizable coordinates
- 🏷️ **Text Watermarking** - Add text overlays with full color and positioning control
- 🔒 **Security First** - Built-in origin and file type validation
- 🐳 **Container Ready** - Optimized Docker images for easy deployment
- 📊 **Health Monitoring** - Built-in health checks and monitoring endpoints
- 🎯 **High Performance** - Powered by SkiaSharp for optimal image processing
- 🔧 **Extensible** - Plugin-based architecture for multiple image processing providers

## 📋 Table of Contents

- [🏗️ Architecture](#️-architecture)
- [🚀 Quick Start](#-quick-start)
- [🛠️ Development Setup](#️-development-setup)
- [📖 API Documentation](#-api-documentation)
- [⚙️ Configuration](#️-configuration)
- [🐳 Docker Deployment](#-docker-deployment)
- [🧪 Testing](#-testing)
- [🗺️ Roadmap](#️-roadmap)
- [📝 Changelog](#-changelog)
- [👥 Contributing](#-contributing)
- [📄 License](#-license)

## 🏗️ Architecture

Imagize.NET follows a clean, modular architecture:

```
📦 Imagize.NET
├── 🎯 Imagize                    # Main Web API project
│   ├── Controllers/              # API controllers
│   ├── Services/                # Application services
│   └── Program.cs               # Application entry point
├── 🧩 Imagize.Abstractions      # Interfaces and contracts
│   ├── IImageTools.cs           # Image processing interface
│   ├── IHttpTools.cs            # HTTP utilities interface
│   └── IExifTools.cs            # EXIF data interface
├── ⚙️ Imagize.Core              # Core business logic
│   ├── HttpTools.cs             # HTTP client utilities
│   ├── Health.cs                # Health check implementation
│   └── Extensions/              # Extension methods
├── 🖼️ Imagize.Providers.SkiaSharp # SkiaSharp implementation
│   └── SkiaSharpImageTools.cs   # Image processing implementation
├── 🧪 ImagizeTests              # Unit tests
└── 📊 ImagizeBenchmarking       # Performance benchmarks
```

### 🔧 Key Components

- **Controllers**: RESTful API endpoints for image operations
- **Abstractions**: Interfaces enabling provider-agnostic design
- **Core**: Shared utilities and business logic
- **Providers**: Pluggable image processing implementations
- **Tests**: Comprehensive unit test suite
- **Benchmarking**: Performance measurement tools

## 🚀 Quick Start

### Using Docker (Recommended)

```bash
# Pull the latest image
docker pull captivereality/imagize:latest

# Run with basic configuration
docker run -d \
  --name imagize \
  -p 8080:80 \
  -e IMAGIZE_ALLOWED_FILETYPES="jpg,jpeg,png,gif,webp" \
  -e IMAGIZE_ALLOWED_ORIGINS="https://your-domain.com,https://cdn.example.com" \
  captivereality/imagize:latest

# Test the service
curl "http://localhost:8080/api/image/resize?uri=https://example.com/image.jpg&width=300&height=200"
```

### Using Docker Compose

```yaml
version: '3.8'
services:
  imagize:
    image: captivereality/imagize:latest
    container_name: imagize
    restart: unless-stopped
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - IMAGIZE_ALLOWED_FILETYPES=jpg,jpeg,png,gif,webp,heic,heif
      - IMAGIZE_ALLOWED_ORIGINS=https://your-cdn.com,https://your-website.com
      - IMAGIZE_TEXT=DEFAULT  # or RICHTEXTKIT for advanced text rendering
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:80/health"]
      interval: 30s
      timeout: 10s
      retries: 3
    deploy:
      resources:
        limits:
          memory: 512M
        reservations:
          memory: 256M
```

## 🛠️ Development Setup

### Prerequisites

- 🔧 **.NET 6.0 SDK** or later
- 💻 **Visual Studio 2022**, **VS Code**, or **JetBrains Rider**
- 🐳 **Docker** (optional, for containerized development)
- 🌐 **Git**

### Clone and Build

```bash
# Clone the repository
git clone https://github.com/captivereality/imagize.git
cd imagize

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run the application
dotnet run --project Imagize
```

### 🐳 Development with Docker

```bash
# Build local Docker image
docker build -t imagize-dev .

# Run with development settings
docker run -d \
  --name imagize-dev \
  -p 5000:80 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e IMAGIZE_ALLOWED_ORIGINS="*" \
  -e IMAGIZE_ALLOWED_FILETYPES="jpg,jpeg,png,gif" \
  imagize-dev
```

## 📖 API Documentation

Once running, visit the **Swagger UI** at: `http://localhost:8080/swagger`

### 🔄 Resize Images

**Endpoint:** `GET /api/image/resize`

```bash
# Basic resize
curl "http://localhost:8080/api/image/resize?uri=https://example.com/photo.jpg&width=800&height=600"

# Resize maintaining aspect ratio
curl "http://localhost:8080/api/image/resize?uri=https://example.com/photo.jpg&width=800&maintainAspectRatio=true"

# Resize with quality control
curl "http://localhost:8080/api/image/resize?uri=https://example.com/photo.jpg&width=400&imageQuality=High"
```

**Parameters:**
- `uri` *(required)*: Source image URL
- `width`: Target width (0 = auto)
- `height`: Target height (0 = auto)
- `imageQuality`: `Low`, `Medium`, `High`, `Maximum`
- `maintainAspectRatio`: `true`/`false` (default: `true`)
- `autoRotate`: `true`/`false` (default: `true`)

### ✂️ Crop Images

**Endpoint:** `GET /api/image/crop`

```bash
# Crop with coordinates
curl "http://localhost:8080/api/image/crop?uri=https://example.com/photo.jpg&left=100&top=50&right=500&bottom=400"
```

**Parameters:**
- `uri` *(required)*: Source image URL
- `left`: Left coordinate (pixels)
- `top`: Top coordinate (pixels)
- `right`: Right coordinate (pixels)
- `bottom`: Bottom coordinate (pixels)

### 🏷️ Add Text Watermarks

**Endpoint:** `GET /api/image/watermarktext`

```bash
# Basic text watermark
curl "http://localhost:8080/api/image/watermarktext?uri=https://example.com/photo.jpg&text=Copyright%202024&x=10&y=10"

# Styled watermark
curl "http://localhost:8080/api/image/watermarktext?uri=https://example.com/photo.jpg&text=My%20Brand&x=50&y=50&textSize=24&red=255&green=0&blue=0&alpha=180"
```

**Parameters:**
- `uri` *(required)*: Source image URL
- `text` *(required)*: Watermark text
- `x`: X position (default: 0)
- `y`: Y position (default: 0)
- `textSize`: Font size (default: 20)
- `canvasOrigin`: `TopLeft`, `TopRight`, `BottomLeft`, `BottomRight`
- `red`, `green`, `blue`: Color values (0-255)
- `alpha`: Transparency (0-255, default: 128)

### 🏥 Health Check

**Endpoint:** `GET /health`

```bash
curl "http://localhost:8080/health"
```

Returns service health status and startup time.

## ⚙️ Configuration

### Environment Variables

| Variable | Description | Example | Required |
|----------|-------------|---------|----------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Development`, `Production` | ✅ |
| `IMAGIZE_ALLOWED_FILETYPES` | Supported image formats | `jpg,jpeg,png,gif,webp` | ✅ |
| `IMAGIZE_ALLOWED_ORIGINS` | Allowed source domains | `https://cdn.example.com,https://images.site.com` | ✅ |
| `IMAGIZE_TEXT` | Text rendering engine | `DEFAULT`, `RICHTEXTKIT` | ❌ |
| `ASPNETCORE_URLS` | Binding URLs | `http://+:80` | ❌ |

### 🔒 Security Configuration

**File Type Restrictions:**
```bash
# Supported formats
IMAGIZE_ALLOWED_FILETYPES="jpg,jpeg,png,gif,webp,bmp,tiff,heic,heif"
```

**Origin Restrictions:**
```bash
# Multiple origins (comma, pipe, or tilde separated)
IMAGIZE_ALLOWED_ORIGINS="https://cdn.mysite.com,https://images.example.com"
# or
IMAGIZE_ALLOWED_ORIGINS="https://cdn.mysite.com|https://images.example.com"
# or
IMAGIZE_ALLOWED_ORIGINS="https://cdn.mysite.com~https://images.example.com"
```

### 🎨 Text Rendering Options

- **`DEFAULT`**: Basic text rendering (cross-platform)
- **`RICHTEXTKIT`**: Advanced text rendering with better font support

## 🐳 Docker Deployment

### Production Deployment

```yaml
version: '3.8'
services:
  imagize:
    image: captivereality/imagize:latest
    restart: unless-stopped
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - IMAGIZE_ALLOWED_FILETYPES=jpg,jpeg,png,gif,webp
      - IMAGIZE_ALLOWED_ORIGINS=https://your-production-cdn.com
    deploy:
      resources:
        limits:
          memory: 1G
          cpus: '1.0'
        reservations:
          memory: 512M
          cpus: '0.5'
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:80/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"
```

### Kubernetes Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: imagize
spec:
  replicas: 3
  selector:
    matchLabels:
      app: imagize
  template:
    metadata:
      labels:
        app: imagize
    spec:
      containers:
      - name: imagize
        image: captivereality/imagize:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: IMAGIZE_ALLOWED_FILETYPES
          value: "jpg,jpeg,png,gif,webp"
        - name: IMAGIZE_ALLOWED_ORIGINS
          value: "https://your-cdn.com"
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 5
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: imagize-service
spec:
  selector:
    app: imagize
  ports:
  - protocol: TCP
    port: 80
    targetPort: 80
  type: LoadBalancer
```

## 🧪 Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test ImagizeTests/

# Run benchmarks
dotnet run --project ImagizeBenchmarking --configuration Release
```

### 🔍 Manual Testing

```bash
# Test resize endpoint
curl -o resized.jpg "http://localhost:8080/api/image/resize?uri=https://httpbin.org/image/jpeg&width=300&height=200"

# Test crop endpoint
curl -o cropped.jpg "http://localhost:8080/api/image/crop?uri=https://httpbin.org/image/jpeg&left=50&top=50&right=250&bottom=200"

# Test watermark endpoint
curl -o watermarked.jpg "http://localhost:8080/api/image/watermarktext?uri=https://httpbin.org/image/jpeg&text=Test%20Watermark&x=10&y=10&textSize=20"

# Test health endpoint
curl "http://localhost:8080/health"
```

## 🗺️ Roadmap

### ✅ Completed Features
- [x] 🖼️ **SkiaSharp Provider** - Primary image processing engine
- [x] 🔄 **Image Resize** - Smart resizing with aspect ratio control
- [x] 🏥 **Server Health** - Health monitoring endpoints
- [x] 📊 **Image Exif** - Complete EXIF data handling
- [x] ✂️ **Crop Image** - Precise image cropping
- [x] 🏷️ **Watermark Text** - Text overlay capabilities

### 🚧 In Progress
- [ ] 🔍 **Zoom Image** - Image zoom and pan functionality
- [ ] 🔄 **Auto Rotate** - Automatic orientation correction
- [ ] 🌀 **Rotate Image** - Manual rotation controls
- [ ] 🌫️ **Blur Image** - Gaussian and motion blur effects

### 🔮 Future Plans
- [ ] 🧠 **Smart Cropping** - AI-powered intelligent cropping
- [ ] 🔗 **Operation Chaining** - Combine multiple operations in single request
- [ ] 🖼️ **Image Watermarks** - Logo and image overlay support
- [ ] 🎨 **Filters & Effects** - Instagram-style filters
- [ ] 📐 **Format Conversion** - Convert between image formats
- [ ] 🔧 **Additional Providers** - ImageSharp, Magick.NET support
- [ ] 📊 **Analytics** - Usage metrics and performance monitoring
- [ ] 🔐 **Authentication** - API key and JWT support
- [ ] 📦 **Batch Processing** - Process multiple images in single request

## 📝 Changelog

### v0.0.3 (2022-07-09)
- ✨ Added basic text watermarking functionality
- 🐛 Fixed Windows-specific text rendering issues
- 📚 Improved API documentation

### v0.0.2 (2022-07-09)
- ✨ Added rudimentary image cropping
- 🔧 Enhanced error handling
- 🧪 Added unit tests for cropping functionality

### v0.0.1 (2022-06-26)
- 🎉 Initial release
- 🔄 Basic image resizing functionality
- 🐳 Docker container support
- 🏥 Health check endpoints
- 📊 Swagger API documentation

## 👥 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### 🛠️ Development Workflow

1. **Fork** the repository
2. **Create** a feature branch: `git checkout -b feature/amazing-feature`
3. **Commit** your changes: `git commit -m 'Add amazing feature'`
4. **Push** to the branch: `git push origin feature/amazing-feature`
5. **Open** a Pull Request

### 🧪 Testing Requirements

- All new features must include unit tests
- Maintain minimum 80% code coverage
- Follow existing code style and conventions
- Update documentation for API changes

## 👨‍💻 Authors

| Name | Role | Contact |
|------|------|----------|
| **Mark Castle** | Creator & Maintainer | [GitHub](https://github.com/captivereality) |

## 📄 License

**MIT License** - see the [LICENSE](LICENCE.txt) file for details.

---

<div align="center">

**Made with ❤️ by [Captive Reality Ltd](https://github.com/captivereality)**

*© 2022 Captive Reality Ltd. All Rights Reserved.*

</div>