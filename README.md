# vBenchSLAM
### The benchmark for Visual SLAM frameworks.


The number of Visual SLAM algorithms grows, alongside them also increases the time to find the most suitable framework for the use case. vBenchSLAM saves time on testing multiple frameworks and takes off the need to worry about the incompatibility of packages and the headache around installing each of them.


## Requirements
To run the vBenchSLAM the following set of requirements has to be met. 

The vBenchSLAM application is the program designed to run primarily under the Linux environment and under the tests have been conducted under the said operating system. Although the application can launch under any system, the application will not work correctly under the Windows environment. The macOS has not been tested yet. 

### Nvidia graphics card
Currently all the implemented SLAM algorithms require the use of the Nvidia Graphics Card and it is the needed component. The driver version should be fairly recent to include the nvidia-smi tool. The drivers installation guide can be accessed using the following [link](https://docs.nvidia.com/datacenter/tesla/tesla-installation-notes/index.html).

### Docker engine
The next tool is the Docker engine. The docker engine is required to download and to run containers with the SLAM algorithms. The installation guide of the docker engine can be found under the following [link](https://docs.docker.com/engine/install/).

### NVIDIA Container Toolkit
After the docker engine and the Nvidia drivers are installed the additional package NVIDIA Container Toolkit has to be installed to allow the access to the Nvidia GPU from the container. The installation guide can be accessed with the following [link](https://docs.nvidia.com/datacenter/cloud-native/container-toolkit/install-guide.html).

### Pangolin Viewer
To display the output from the Docker container the installation of the Pangolin Viewer is required. The installation process of a Pangolin Viewer is described in its [repository](https://github.com/stevenlovegrove/Pangolin). Additionally the X11 needs to be installed to forward the image from the docker container to the client’s machine.

### .NET SDK
The .NET SDK is required for building and publishing the application. The instruction on installing the SDK is placed in the Microsoft Docs website under the following [link](https://docs.microsoft.com/en-us/dotnet/core/install/linux). 

## Build and publish

To build and publish (create the executable) the vBenchSLAM application the steps described have to be taken. To build the project the command line approach will be used. 
Clone the project from the repository or use the existing copy of the code that is available on the machine. The code can be cloned from the vBenchSLAM repository on GitHub.
After the code is cloned enter the newly created folder. To build the project open the terminal in the catalogue and type the command 

> `dotnet build –configuration Release`

to build the project in the Release configuration. If the command is run for the first time it takes longer as it has to restore the necessary NuGet packages that are required by the vBenchSLAM to run. After the restoring process is finished, the binaries are compiled. The next step is to publish the binaries. This is done by the command:

> `dotnet publish --configuration Release -r linux-x64 --self-contained`

