# MSFS-FSSimConnectorLib
Library to communicate with MSFS 2020 to retrieve variables and send events both asynchronously and synchronously using the library built-in engine. 

## General features
 * Retrieve variables freely, without engine (see note=
 * Send events freely, without engine (see note)
 * Engine

NOTE: using this library without engine cannot asure you that you will be free of racing conditions or events being executed before a previous variable request. In order to have control over this, please use the engine. These are internal library methods that are exposed just in case you want to play with it.

## Engine features
 * Retrieve variables
 * Send events
 * Execute QuickActions
 * Execute automations (main use of the engine is intended for this purpose)

NOTE: awaiting (using *await*) all of them is highly recommended.

### Available engine features

