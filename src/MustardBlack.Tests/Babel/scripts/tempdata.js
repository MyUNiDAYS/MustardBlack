(function () {

	window.tempData = {};

	window.setTempData = function (data) {
		cookie('temp', data, {
			domain: document.domain,
			path: '/'
		});
	}

	// Read temp data
	var temp = cookie('temp');
	
	if (!temp)
		return;

	try
	{
		window.tempData = JSON.parse(temp);		
	} catch (e) { }

	// Clear temp data
	window.setTempData(null);

}());
