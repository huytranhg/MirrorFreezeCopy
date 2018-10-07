// <copyright file="ProjectInstaller.cs" company="Huy Tran">
// Copyright (c) Huy Tran. All rights reserved.
// </copyright>

namespace MirrorFreezeCopy.WindowsService
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// ProjectInstaller.
    /// </summary>
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectInstaller"/> class.
        /// ProjectInstaller constructor.
        /// </summary>
        public ProjectInstaller()
        {
            this.InitializeComponent();
        }
    }
}
