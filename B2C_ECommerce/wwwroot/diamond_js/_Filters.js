$(function () {
	GetShapeDesign();
	GetCarat();
	var fixColors = GetColorData();
	var fixCut = GetCutData();
	var fixClarity = GetClarityData();

	var diamondFilters =
	{
		Colors: $('#colors').val() ? $('#colors').val() : [],
		Carats: $('#carats').val() ? $('#carats').val() : [],
		Shapes: $('#shapes').val() ? $('#shapes').val() : [],
		Clarities: $('#clarities').val() ? $('#clarities').val() : [],
		Prices: $('#prices').val() ? $('#prices').val() : [],
		Ratios: $('#ratios').val() ? $('#ratios').val() : [],
		Tables: $('#tables').val() ? $('#tables').val() : [],
		Depthes: $('#depthes').val() ? $('#depthes').val() : [],
		Polishes: $('#polishes').val() ? $('#polishes').val() : [],
		Fluors: $('#fluors').val() ? $('#fluors').val() : [],
		Symmetries: $('#symmetries').val() ? $('#symmetries').val() : []
	};
	getDiamondDataList(diamondFilters, 1, 10);

	// var fixColors = ["D", "E", "F"];
	var fixClarity = ["IF", "VVS1", "VVS2", "VS1", "VS2"];
	var fixCut = ["IDEAL", "EXCELLENT", "VERY GOOD", "GOOD", "FAIR"];

	var fixPolish = ["IDEAL", "EXCELLENT", "VERY GOOD", "GOOD", "FAIR"];
	var fixFluor = ["IDEAL", "EXCELLENT", "VERY GOOD", "GOOD", "FAIR"];
	var fixSym = ["IDEAL", "EXCELLENT", "VERY GOOD", "GOOD", "FAIR"];


	$("#btnMoreFilter").click(function () {
		$("#divMoreFilter").show();
		$("#btnLessFilter").show();
		$("#btnMoreFilter").hide();
	});

	$("#btnLessFilter").click(function () {
		$("#divMoreFilter").hide();
		$("#btnLessFilter").hide();
		$("#btnMoreFilter").show();
	});

	$("#price-slider").slider({
		range: true,
		min: 500,
		max: 50000,
		step: 0.01,
		values: [5000, 30000],
		slide: function (event, ui) {
			$("#minPrice").val("$" + ui.values[0]);
			$("#maxPrice").val("$" + ui.values[1]);
		}
	});

	$("#Cut-slider").slider({
		range: true,
		min: 0,
		max: fixCut.length - 1,
		step: 1,
		values: [0, 1],
		slide: function (event, ui) {
			$("#minCut").val(fixCut[ui.values[0]]);
			$("#maxCut").val(fixCut[ui.values[1]]);
		}
	});

	$("#Clarity-slider").slider({
		range: true,
		min: 0,
		max: fixClarity.length - 1,
		step: 1,
		values: [0, 1],
		slide: function (event, ui) {
			$("#minClarity").val(fixClarity[ui.values[0]]);
			$("#maxClarity").val(fixClarity[ui.values[1]]);
		}
	});

	$("#Polish-slider").slider({
		range: true,
		min: 0,
		max: fixPolish.length - 1,
		step: 1,
		values: [0, 1],
		slide: function (event, ui) {
			$("#minPolish").val(fixPolish[ui.values[0]]);
			$("#maxPolish").val(fixPolish[ui.values[1]]);
		}
	});

	$("#Fluor-slider").slider({
		range: true,
		min: 0,
		max: fixFluor.length - 1,
		step: 1,
		values: [0, 1],
		slide: function (event, ui) {
			$("#minFluor").val(fixFluor[ui.values[0]]);
			$("#maxFluor").val(fixFluor[ui.values[1]]);
		}
	});

	$("#Symmetry-slider").slider({
		range: true,
		min: 0,
		max: fixSym.length - 1,
		step: 1,
		values: [0, 1],
		slide: function (event, ui) {
			$("#minSymmetry").val(fixSym[ui.values[0]]);
			$("#maxSymmetry").val(fixSym[ui.values[1]]);
		}
	});

	$("#minPrice").val("$ " + $("#price-slider").slider("values", 0));
	$("#maxPrice").val("$ " + $("#price-slider").slider("values", 1));
	$("#minCaratSize").val($("#carat-size-slider").slider("values", 0));
	$("#maxCaratSize").val($("#carat-size-slider").slider("values", 1));

	$("#minColor").val(fixColors[$("#color-slider").slider("values", 0)]);
	$("#maxColor").val(fixColors[$("#color-slider").slider("values", 1)]);

	$("#minCut").val(fixCut[$("#Cut-slider").slider("values", 0)]);
	$("#maxCut").val(fixCut[$("#Cut-slider").slider("values", 1)]);

	$("#minClarity").val(fixClarity[$("#Clarity-slider").slider("values", 0)]);
	$("#maxClarity").val(fixClarity[$("#Clarity-slider").slider("values", 1)]);

	$("#minPolish").val(fixPolish[$("#Polish-slider").slider("values", 0)]);
	$("#maxPolish").val(fixPolish[$("#Polish-slider").slider("values", 1)]);

	$("#minFluor").val(fixFluor[$("#Fluor-slider").slider("values", 0)]);
	$("#maxFluor").val(fixFluor[$("#Fluor-slider").slider("values", 1)]);

	$("#minSymmetry").val(fixSym[$("#Symmetry-slider").slider("values", 0)]);
	$("#maxSymmetry").val(fixSym[$("#Symmetry-slider").slider("values", 1)]);
});

async function GetShapeDesign() {
	const shapeListContainer = $('#filter-box #shape-list'); // Cache the DOM selector
	shapeListContainer.empty(); // Clear the previous content

	try {
		const response = await fetch('@SD.BaseApiUrl/api/diamondproperty/get-shape-list'); // Using native fetch
		const data = await response.json();

		if (Array.isArray(data) && data.length > 0) {
			const shapeButtons = data.map(shape => {
				return `
					  <div class="shape-btn text-center">
						<img src="${shape.IconPath}" alt="${shape.Name}" />
						<span>${shape.Name}</span>
					  </div>
					`;
			}).join(''); // Concatenate all the buttons in a single string

			shapeListContainer.html(shapeButtons); // Append all the shapes at once
		} else {
			shapeListContainer.html('<p>No shapes available</p>');
		}
	} catch (error) {
		console.error('Error fetching data:', error);
		shapeListContainer.html('<p>Failed to load shapes</p>');
	}
}


async function GetCarat() {
	try {
		var response = await $.ajax({
			url: '@(SD.BaseApiUrl)/api/diamondproperty/get-carat-ranges', // API endpoint
			method: 'GET', // Using GET request
			dataType: 'json', // Expecting JSON response
		});

		// Initialize the slider with API response
		$("#carat-size-slider").slider({
			range: true,
			min: response.MinCaratSize,  // Set minimum from API response
			max: response.MaxCaratSize,  // Set maximum from API response
			step: 0.1, // Adjust step size as needed
			values: [response.MinCaratSize, response.MaxCaratSize / 2], // Default values
			slide: function (event, ui) {
				$("#minCarat").val(ui.values[0]);
				$("#maxCarat").val(ui.values[1]);
			}
		});

		// Set initial values
		$("#minCarat").val(response.MinCaratSize);
		$("#maxCarat").val(response.MaxCaratSize / 2);

	} catch (error) {
		console.error('Error fetching data: ', error);
		$('#filter-box #shape-list').html('<p>Failed to load shapes</p>');
	}
}

async function GetColorData() {
	try {
		// Fetching data from API
		var response = await $.ajax({
			url: '@(SD.BaseApiUrl)/api/diamondproperty/diamond-property/get-color-list',
			method: 'GET',
			dataType: 'json',
		});

		// Extract color names
		const colorList = response.map(item => item.Name);

		// Ensure that there's at least one color to avoid errors
		if (colorList.length === 0) {
			console.warn('No colors available');
			return;
		}

		// Calculate slider step as the index difference
		const StepCount = 1;

		$("#color-slider").slider({
			range: true,
			min: 0,
			max: colorList.length - 1,
			step: StepCount,
			values: [0, colorList.length - 1],
			slide: function (event, ui) {
				// Set the selected color values based on slider indices
				$("#minColor").val(colorList[ui.values[0]]);
				$("#maxColor").val(colorList[ui.values[1]]);
			}
		});

		// Set initial values for the min and max color fields
		$("#minColor").val(colorList[0]);
		$("#maxColor").val(colorList[colorList.length - 1]);

	} catch (error) {
		console.error('Error fetching data: ', error);
		$('#filter-box #color-slider').html('<p>Failed to load colors</p>');
	}
}

async function GetCutData() {
	try {
		// Fetching data from the API
		const response = await $.ajax({
			url: '@(SD.BaseApiUrl)/api/diamondproperty/get-cut-list',
			method: 'GET',
			dataType: 'json',
		});

		// Check if response is valid and contains the necessary data
		if (!Array.isArray(response) || response.length === 0) {
			console.warn('No cuts available');
			$('#filter-box #cut-slider').html('<p>No cuts available</p>');
			return;
		}

		// Extract the cut names directly from the response
		const cutList = response.map(item => item.Name);

		// Ensure there is at least one cut value
		if (cutList.length === 0) {
			console.warn('No cuts available');
			return;
		}

		// Initialize the slider with the extracted data
		const minCut = cutList[0];
		const maxCut = cutList[cutList.length - 1];

		// Calculate the step count (fixed to 1 as per your code)
		const StepCount = 1;

		$("#cut-slider").slider({
			range: true,
			min: 0,
			max: cutList.length - 1,
			step: StepCount,
			values: [0, cutList.length - 1],
			slide: function (event, ui) {
				// Update the min and max values dynamically based on the slider
				$("#minCut").val(cutList[ui.values[0]]);
				$("#maxCut").val(cutList[ui.values[1]]);
			}
		});

		// Set initial values for the min and max cut fields
		$("#minCut").val(minCut);
		$("#maxCut").val(maxCut);

	} catch (error) {
		// Handle errors during the API call or slider setup
		console.error('Error fetching data: ', error);
		$('#filter-box #cut-slider').html('<p>Failed to load cuts</p>');
	}
}

async function GetClarityData() {
	try {
		// Fetching data from the API
		const response = await $.ajax({
			url: '@(SD.BaseApiUrl)/api/diamondproperty/diamond-property/get-clarity-list',
			method: 'GET',
			dataType: 'json',
		});

		// Check if response is valid and contains the necessary data
		if (!Array.isArray(response) || response.length === 0) {
			console.warn('No Clarity available');
			$('#filter-box #Clarity-slider').html('<p>No clarity available</p>');
			return;
		}

		// Extract the cut names directly from the response
		const clarityList = response.map(item => item.Name);

		// Ensure there is at least one cut value
		if (clarityList.length === 0) {
			console.warn('No clarity available');
			return;
		}

		// Initialize the slider with the extracted data
		const minClarity = clarityList[0];
		const maxClarity = clarityList[clarityList.length - 1];

		// Calculate the step count (fixed to 1 as per your code)
		const StepCount = 1;

		$("#Clarity-slider").slider({
			range: true,
			min: 0,
			max: clarityList.length - 1,
			step: StepCount,
			values: [0, clarityList.length - 1],
			slide: function (event, ui) {
				// Update the min and max values dynamically based on the slider
				$("#minClarity").val(clarityList[ui.values[0]]);
				$("#maxClarity").val(clarityList[ui.values[1]]);
			}
		});

		// Set initial values for the min and max cut fields
		$("#minClarity").val(minClarity);
		$("#maxClarity").val(maxClarity);

	} catch (error) {
		// Handle errors during the API call or slider setup
		console.error('Error fetching data: ', error);
		$('#filter-box #cut-slider').html('<p>Failed to load clarity</p>');
	}
}





async function getDiamondDataList(diamondFilters, pageNumber = 1, pageSize = 10) {
	return $.ajax({
		url: '/diamond/GetDiamondList', // Change this to your actual route
		type: 'POST',
		data: {
			diamondFilters: diamondFilters,
			pageNumber: pageNumber,
			pageSize: pageSize
		},
		success: function (data) {
			$('#diamond-list-container').html(data);
		},
		error: function (error) {
			console.error('Error fetching data:', error);
		}
	});
}

function ShowCertificate(certificate) {
	$.ajax({
		url: '@Url.Action("GetCertificate", "Diamond", new { diamondCerti = "__CERTIFICATE__" })'.replace('__CERTIFICATE__', certificate),
		type: 'GET',
		dataType: 'json',
		success: function (response) {
			$('#dimCertiModal').html(response);
			$('#dimCertiModal').modal('show');
		},
		error: function (xhr, status, error) {
			console.error('AJAX error:', status, error);
		}
	});
}