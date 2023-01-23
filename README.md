# Client/Multi-Client and Server Networking configuration in C# MVVM WPF
A Multi-client-server networking system built on top of Payload's YouTube video, added some extra events and functionalities along with encryption and proper error handling. Thanks to Payload for the project base! :D

Both client and server use symmetric key encryption using the AES encryption algorithm to encrypt all messages and data transmissions. IV for the encryption algorithm is changed for every message and sent along with the message meaning two identical message produce completely different encrypted outputs. In order to crack the message, an attacker would need the message IV and the public key of the server or client 

Private and public Keys are calculated via ECDH (Elliptic-curve Diffie–Hellman) and exchanged via port 443 TLS (transport layer security) to ensure a secure key exchange between the client and server
