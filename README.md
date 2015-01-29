# What's this for?

This is a Visual Studio 2012+ extension which automatically triggers all T4 templates in your solution (or selected project for project-only builds) to be re-run before build.

# Installation

Just build and install the template using the VSIX file, [grab it from the Visual Studio gallery](http://visualstudiogallery.msdn.microsoft.com/84e6f033-6da3-4641-a058-12feef0a33b9) or **do it the easy way** and use the 'Extension & Updates' manager in Visual Studio 2012+.

# Disabling individual templates

By default, the template will run all of your T4 templates on build. If you want to turn off individual templates, you can do this by setting **Run on build** to `false` in the _Properties window_ for any `.tt` file in your solution. 