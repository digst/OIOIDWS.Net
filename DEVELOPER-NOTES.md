Developer Notes
===============

## Creating a new OIOIDWS.Net release

### Update documentation

Ensure documentation is updated with the changes.

### Run tests

All unit and integration tests must pass prior to releasing.

### Build the packages
* Run the `BuildPackages.ps1`, setting version to a proper version number, 
  e.g `1.0.0` and assemblyVersion to a proper version number, e.g `1.0.0.0`.
  Use `1.0.0-alpha`, `1.00-beta`, etc. as version number to make a pre-release.
* Verify the packages looks good and are ready to publish
* Ensure API key to digitaliseringsstyrelsen's nuget account is installed on your machine
* Push packages to NuGet by running `BuildPackages.ps1` with the switch `-pushPackages`
* Add a tag in Subversion corresponding to the release

### Creating the new resource on digitaliser.dk
* Login on [digitaliser.dk][digitaliser] and go to the newest version of the
  [ressource][ressource].
* Choose Funktioner and click on `Opret ny version`.
* Change the metadata of the ressource accordingly, remember adding link to 
  `SVN` and the published Nuget packages.
* Publish the new version when ready. 

[digitaliser]: https://digitaliser.dk/
[ressource]: http://digitaliser.dk/resource/2513426

### Change the frontpage of the group
* Login on digitaliser.dk and go to the [OIOIDWS group][group].
* Find the old promotion on the grouppage and remove it.
* Find the new promotion on the grouppage by using the ID from the URL of the
  page showing the new version.

[group]: http://digitaliser.dk/group/705156