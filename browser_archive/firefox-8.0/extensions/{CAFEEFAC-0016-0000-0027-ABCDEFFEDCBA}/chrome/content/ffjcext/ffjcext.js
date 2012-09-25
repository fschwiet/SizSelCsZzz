const gJavaConsole1_6_0_27 =  {

	id	: "javaconsole1.6.0_27",

	mimeType: "application/x-java-applet;jpi-version=1.6.0_27",

	install	: function() {
		window.addEventListener("load",this.init,false);
	},

	init	: function() { 
		if (navigator.mimeTypes[gJavaConsole1_6_0_27.mimeType]) {
			var toolsPopup = document.getElementById("menu_ToolsPopup");	
			toolsPopup.addEventListener("popupshowing",gJavaConsole1_6_0_27.enable,false);
			var element = document.getElementById(gJavaConsole1_6_0_27.id);
			element.setAttribute( "oncommand" , "gJavaConsole1_6_0_27.show();");
		} else {
			var element = document.getElementById(gJavaConsole1_6_0_27.id);
			element.setAttribute("style", "display: none");
		}
	},

	enable	: function() {
		var element = document.getElementById(gJavaConsole1_6_0_27.id);
    		if (navigator.javaEnabled()) {
			element.removeAttribute("disabled");
    		} else {
      			element.setAttribute("disabled", "true");
    		}
	},

	show	: function() {
     		var jvmMgr = Components.classes['@mozilla.org/oji/jvm-mgr;1']
	                   .getService(Components.interfaces.nsIJVMManager)
    		jvmMgr.showJavaConsole();
	}
	
};

gJavaConsole1_6_0_27.install();


