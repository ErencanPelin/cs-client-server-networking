# Client/Multi-Client and Server Networking configuration in C# MVVM WPF
A Multi-client-server networking system built on top of Payload's YouTube video, added some extra events and functionalities along with encryption and proper error handling. Thanks to Payload for the project base! :D

Both client and server use symmetric key encryption using the AES encryption algorithm to encrypt all messages and data transmissions. IV for the encryption algorithm is changed for every message and sent along with the message meaning two identical messages produce completely different cipher outputs. In order to crack the message, an attacker would need the message IV and the public key of the server or client.
<br><br>
# Changelog
In order of latest changes & using semantic versioning.

## [0.1.0] - 28-01-2023
### Added
- AES encryption on client and server end (symmetric key encryption)
- C# events to handle network events (Client join, leave, server start, message receive) events occur on both client and server side in case they want to perform different things during those events
- some code optimisations

## [0.0.1] - 15-01-2023
### Added
- Server console app
- Client WPF app
- initial upload
- Following Payload's YouTube tutorial
