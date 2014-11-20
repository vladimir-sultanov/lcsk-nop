var browserInfo =
{
    nVer : navigator.appVersion,
    nAgt : navigator.userAgent,
    browserName : navigator.appName,
    fullVersion : '' + parseFloat(navigator.appVersion),
    majorVersion : parseInt(navigator.appVersion, 10),
    nameOffset : '',
    verOffset : '',
    ix: ''
}

// In Opera, the true version is after "Opera" or after "Version"

if ((browserInfo.verOffset = browserInfo.nAgt.indexOf("Opera")) != -1) {
    browserInfo.browserName = "Opera";
    browserInfo.fullVersion = browserInfo.nAgt.substring(browserInfo.verOffset + 6);
    if ((browserInfo.verOffset = browserInfo.nAgt.indexOf("Version")) != -1)
        browserInfo.fullVersion = browserInfo.nAgt.substring(browserInfo.verOffset + 8);
}
    // In MSIE, the true version is after "MSIE" in userAgent

else if ((browserInfo.verOffset = browserInfo.nAgt.indexOf("MSIE")) != -1) {
    browserInfo.browserName = "Microsoft Internet Explorer";
    browserInfo.fullVersion = browserInfo.nAgt.substring(browserInfo.verOffset + 5);
}
    // In Chrome, the true version is after "Chrome"

else if ((browserInfo.verOffset = browserInfo.nAgt.indexOf("Chrome")) != -1) {
    browserInfo.browserName = "Chrome";
    browserInfo.fullVersion = browserInfo.nAgt.substring(browserInfo.verOffset + 7);
}
    // In Safari, the true version is after "Safari" or after "Version"

else if ((browserInfo.verOffset = browserInfo.nAgt.indexOf("Safari")) != -1) {
    browserInfo.browserName = "Safari";
    browserInfo.fullVersion = browserInfo.nAgt.substring(browserInfo.verOffset + 7);
    if ((browserInfo.verOffset = browserInfo.nAgt.indexOf("Version")) != -1)
        browserInfo.fullVersion = browserInfo.nAgt.substring(browserInfo.verOffset + 8);
}
    // In Firefox, the true version is after "Firefox"

else if ((browserInfo.verOffset = browserInfo.nAgt.indexOf("Firefox")) != -1) {
    browserInfo.browserName = "Firefox";
    browserInfo.fullVersion = browserInfo.nAgt.substring(browserInfo.verOffset + 8);
}
    // In most other browsers, "name/version" is at the end of userAgent

else if ((browserInfo.nameOffset = browserInfo.nAgt.lastIndexOf(' ') + 1) <
          (browserInfo.verOffset = browserInfo.nAgt.lastIndexOf('/'))) {
    browserInfo.browserName = browserInfo.nAgt.substring(browserInfo.nameOffset, browserInfo.verOffset);
    browserInfo.fullVersion = browserInfo.nAgt.substring(browserInfo.verOffset + 1);
    if (browserInfo.browserName.toLowerCase() == browserInfo.browserName.toUpperCase()) {
        browserInfo.browserName = navigator.appName;
    }
}

// trim the fullVersion string at semicolon/space if present

if ((browserInfo.ix = browserInfo.fullVersion.indexOf(";")) != -1)
    browserInfo.fullVersion = browserInfo.fullVersion.substring(0, browserInfo.ix);
if ((browserInfo.ix = browserInfo.fullVersion.indexOf(" ")) != -1)
    browserInfo.fullVersion = browserInfo.fullVersion.substring(0, browserInfo.ix);

browserInfo.majorVersion = parseInt('' + browserInfo.fullVersion, 10);
if (isNaN(browserInfo.majorVersion)) {
    browserInfo.fullVersion = '' + parseFloat(navigator.appVersion);
    browserInfo.majorVersion = parseInt(navigator.appVersion, 10);
}
//document.write(''
// + 'Browser name  = ' + browserName + '<br>'
// + 'Full version  = ' + fullVersion + '<br>'
// + 'Major version = ' + majorVersion + '<br>'
// + 'navigator.appName = ' + navigator.appName + '<br>'
// + 'navigator.userAgent = ' + navigator.userAgent + '<br>'
//)