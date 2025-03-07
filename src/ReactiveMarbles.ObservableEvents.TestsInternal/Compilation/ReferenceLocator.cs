﻿// Copyright (c) 2019-2023 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using R3.NuGet.Helpers;

namespace R3.SourceGenerator.TestNuGetHelper.Compilation
{
    /// <summary>
    /// Implements the reference locations for windows builds.
    /// </summary>
    public static class ReferenceLocator
    {
        private static readonly PackageIdentity _vsWherePackageIdentity = new("VSWhere", new NuGetVersion("2.6.7"));

        private static readonly ConcurrentDictionary<bool, string> _windowsInstallationDirectory = new();

        /// <summary>
        /// Gets the reference location.
        /// </summary>
        /// <param name="includePreRelease">If we should include pre-release software.</param>
        /// <returns>The reference location.</returns>
        public static string GetReferenceLocation(bool includePreRelease = true)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "/Library/Frameworks";
            }

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new InvalidOperationException(
                    "Visual Studio reference location not supported on this platform: "
                    + RuntimeInformation.OSDescription);
            }

            var visualStudioInstallation = GetWindowsInstallationDirectory(includePreRelease);

            return Path.Combine(visualStudioInstallation, "Common7", "IDE", "ReferenceAssemblies", "Microsoft", "Framework");
        }

        private static string GetWindowsInstallationDirectory(bool includePreRelease) =>
            _windowsInstallationDirectory.GetOrAdd(
                includePreRelease,
                incPreRelease =>
                {
                    return Task.Run(
                        async () =>
                        {
                            var results = await NuGetPackageHelper.DownloadPackageFilesAndFolder(
                                new[] { _vsWherePackageIdentity },
                                new[] { new NuGetFramework("Any") },
                                packageFolders: new[] { PackagingConstants.Folders.Tools },
                                getDependencies: false).ConfigureAwait(false);

                            var fileName = results.IncludeGroup.GetAllFileNames().FirstOrDefault(x => x.EndsWith("vswhere.exe", StringComparison.InvariantCultureIgnoreCase));

                            if (fileName == null)
                            {
                                throw new ReferenceLocationNotFoundException("Cannot find visual studio installation, due to vswhere not being installed correctly.");
                            }

                            var parameters = new StringBuilder("-latest -nologo -property installationPath -format value");

                            if (incPreRelease)
                            {
                                parameters.Append(" -prerelease");
                            }

                            using var process = new Process
                            {
                                StartInfo =
                                {
                                    FileName = fileName,
                                    Arguments = parameters.ToString(),
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true
                                }
                            };

                            process.Start();

                            // To avoid deadlocks, always read the output stream first and then wait.
                            var output = (await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false)).Replace(Environment.NewLine, string.Empty);
                            process.WaitForExit();

                            return output;
                        }).ConfigureAwait(false).GetAwaiter().GetResult();
                });
    }
}
