# LocalImageGeneratorAI

A local AI-powered image generation application built with C# and ASP.NET Core Razor Pages, using ComfyUI as the image generation backend.

## Project Overview

The goal of this project is to build a web application that allows users to generate images locally by entering text prompts.

The application communicates with a locally running ComfyUI instance through its REST API, submits an image generation workflow, retrieves the generated image, and displays it in the browser.

## Current Version – Version 2

Version 2 is a functional implementation of the core image generation process.

### Features

- Enter an image prompt through a web interface.
- Load and modify a ComfyUI API workflow.
- Submit the workflow to ComfyUI using an HTTP POST request.
- Retrieve the generation status through the ComfyUI History API.
- Download the generated image through the ComfyUI View API.
- Display the generated image in the web application.

### Technologies

- C#
- ASP.NET Core
- Razor Pages
- HttpClient
- REST API
- JSON
- ComfyUI
- Z-Image-Turbo

## How It Works

1. The user enters a prompt in the web interface.
2. ASP.NET Core loads the ComfyUI workflow JSON.
3. The prompt is inserted into the workflow.
4. The application sends the workflow to the ComfyUI API.
5. ComfyUI generates the image.
6. The application checks the generation status through the History API.
7. Once generation is complete, the image is retrieved and displayed in the browser.

## Future Development – Version 3

Version 3 will be developed as a completely new, independent ASP.NET Core project.

The goal is to rebuild the functionality of Version 2 from scratch, using the experience gained during its development. This will also serve as a practical exercise to strengthen my understanding of C#, ASP.NET Core, API communication, and application architecture.

After rebuilding the core functionality, additional features will be introduced.

Planned improvements include:

- Selection between multiple AI models.
- Configurable image dimensions and generation settings.
- Negative prompts and seed configuration.
- Image gallery and generation history.
- Improved error handling and logging.
- Application configuration and settings.
- A more structured and maintainable application architecture.

**Version 3 is intended to replace Version 2 as the main project once the new implementation is complete.** Version 2 will remain available as the earlier working implementation and development milestone.
