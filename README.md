# Send Text to Notion

English | [日本語](README.ja.md)

A Flow Launcher plugin that sends text you enter to a new page in a specified Notion database.

## 1. Requirements

- Windows 10 or 11 (x64)
- Flow Launcher 1.19 or later
- Permission to create a Notion Internal Integration

## 2. Installation

1. Download the latest `Flow.Launcher.Plugin.SendTextToNotion.zip` from [GitHub Releases](https://github.com/l7u7ch/flow-launcher-plugin-send-text-to-notion/releases).
2. Extract the ZIP file into any folder under `%APPDATA%\FlowLauncher\Plugins`.
3. Confirm that `plugin.json` is located directly in the extracted folder.
4. Restart Flow Launcher.

## 3. Prepare Notion

1. Create an Internal Integration in Notion under **Settings > My connections > Develop or manage integrations**, then obtain its API token.
2. Open the target database and connect the Integration from **… > Connect to**.
3. Obtain the Database ID from the database URL.
4. Check the name of the database title property.

## 4. Configure the plugin

Open **Settings > Plugins > Send Text to Notion** in Flow Launcher and enter:

- The Notion Internal Integration API Token
- The target Database ID
- The title property name to write to

## 5. Usage

1. Open Flow Launcher.
2. Enter the text you want to send.
3. Select the displayed Notion result and press Enter.

The default action keyword is `*` (global).
