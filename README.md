# Password Reset Brute Force GUI

## Project Description
This project is an Object-Oriented Programming final task developed using .NET MAUI. The application demonstrates password creation, SHA256 hashing with a static salt, single-thread brute force, and multi-thread brute force.

## Features
- Random password creation
- SHA256 password hashing with static salt
- Brute force search from length 1 to 6
- Single-thread brute force
- Multi-thread brute force using Task-based parallel execution
- Dynamic thread selection using a GUI slider
- Thread count selection from 1 to maximum available CPU count
- Start and stop buttons
- Progress indicator
- Elapsed time display
- Found password output
- Performance comparison log

## Version History

### Version 1
Created the .NET MAUI project structure.

### Version 2
Added password generation and SHA256 hashing.

### Version 3
Added brute force generator and password validator.

### Version 4
Added single-thread brute force functionality.

### Version 5
Added multi-thread brute force using Task-based parallel execution.

### Version 6
Added GUI controls, progress display, elapsed time display, and found password output.

### Version 7
Added README file with project description, features, version history, and educational purpose.

### Version 8
Updated README version history.

### Version 9
Added a GUI Slider to dynamically select the number of threads from 1 to the maximum available CPU count. The multi-thread brute-force method now uses the selected slider value instead of a hardcoded thread count.

## Educational Purpose
This application is created for educational purposes only. It demonstrates password hashing and brute force concepts locally within the application.