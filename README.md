# What's this for?

This is a Visual Studio 2012+ extension which automatically triggers all T4 templates in your solution (or selected project for project-only builds) to be re-run before build.

# Installation

Just build and install the template using the VSIX file, [grab it from the Visual Studio gallery](http://visualstudiogallery.msdn.microsoft.com/84e6f033-6da3-4641-a058-12feef0a33b9) or **do it the easy way** and use the 'Extension & Updates' manager in Visual Studio 2012+.

# Configuration

By default, the extension will run all of your T4 templates on build. If that's what you want, great -- **zero configuration required**.

If you want to customize the behaviour, you have two options:

* Change the default behaviour from the _AutoT4_ section of the Visual Studio options dialog.
* Override the default behaviour for a specific template by changing the **Run on build** setting in the _Properties window_ for any `.tt` file in your solution. Whatever you specify here will take precedence over the default in the options dialog (unless you select "Default").