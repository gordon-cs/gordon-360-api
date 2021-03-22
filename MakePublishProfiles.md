# How to create the Publish Profiles to publish the API to the Sites

## Table of Contents

- [Why we need Publish Profiles](#why-we-need-publish-profiles)
- [Why don't we have the publish profiles](#why-dont-we-have-the-publish-profiles)
- [How to create the Publish Profiles](#how-to-create-the-publish-profiles)

## Why we need Publish Profiles

The publish profiles are a tool used by Visual Studio to publish the latest build of a project (in this case, the API). While it is possible to publish manually, it takes more time, is harder to teach, and easier to mess up.

## Why Don't we have the Publish Profiles

There are a few possibilities:
1. You cloned the API into a new location. Since the publish profiles are tracked by Visual Studio and not Git, a new clone of the API won't include the profiles.
2. They somehow got deleted.

## How to Create the Publish Profiles

1. Follow the instructions for deploying to the API site [Deploying to the API site](README.md#deploying-to-the-api-site) until you have to choose a publish profile. If you see the publish page with a DEV or Prod profile, your profiles already exist. You're all set. Otherwise, you will either see the pop up window to pick a publish target, or else you can click start or new profile from the publish menu.
2. Within the "Pick a Publish Target" window, select Folder, and then browse the files. You want to select `Data (F:)` -> `Sites` -> `360Api` or `360ApiTrain` (the former for Prod, the latter for DEV).
3. Your Folder should now be set to `F:\Sites\360Api(Train)`. Select `Advanced` beneath the filepath. If you are making the DEV profile, set the configuration to Debug. Otherwise, leave it as Release for Prod.
4. Click `Create Profile`.
5. Finally, click the `Actions` drop down and rename profile to either DEV or Prod.

Now, you have your publish profiles. Simply select the one you want to use and click `Publish`.
