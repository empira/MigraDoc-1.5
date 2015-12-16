# MigraDoc
MigraDoc Foundation - Creating documents on the fly

MigraDoc references PDFsharp as a submodule. After pulling MigraDoc to a local repository, call
* git submodule init
* git submodule update

to update the submodule.

When forking MigraDoc, the fork will still reference the original PDFsharp repository. Consider forking PDFsharp, too, and use your fork as a submodule.

When downloading MigraDoc as a ZIP, the submodule PDFsharp will be empty. So also download a ZIP for the PDFsharp repository.


Please note: Source code is also available on CodePlex as a ZIP file. The MigraDoc ZIP file on CodePlex does include the PDFsharp files.
