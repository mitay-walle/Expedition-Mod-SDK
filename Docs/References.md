# References

The SDK design was informed by:

- [Introduction to Modding Unity Games With Addressables](https://www.kodeco.com/14494028-introduction-to-modding-unity-games-with-addressables) вЂ” the separate Unity project and secondary-catalog workflow.
- [Unity Addressables: Build content in a separate project](https://docs.unity3d.com/Packages/com.unity.addressables@2.11/manual/MultiProject.html) вЂ” official multi-project requirements and version compatibility.
- [Unity Addressables: Load additional catalogs](https://docs.unity3d.com/Packages/com.unity.addressables@2.11/manual/LoadContentCatalogAsync.html) вЂ” runtime secondary-catalog loading and catalog lifetime.
- [Unity Addressables: Asset dependencies](https://docs.unity3d.com/Packages/com.unity.addressables@2.11/manual/AssetDependencies.html) вЂ” why referenced runtime types must already exist in the game.
- [Unity managed code stripping](https://docs.unity3d.com/Manual/managed-code-stripping.html) вЂ” why Player stripping is not used to manufacture SDK reference assemblies.
- [Unity Addressables sample repository](https://github.com/Unity-Technologies/Addressables-Sample) вЂ” reference examples only; its README explicitly says samples are not guaranteed to be tested or maintained.

Project decisions and limitations are recorded in [Architecture](Architecture.md).
