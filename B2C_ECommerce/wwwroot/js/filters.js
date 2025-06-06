﻿
$(document).ready(function () {
	var page = 1;
	var rangeVal = 10;
	// Initialize filters
	const getValue = id => $(id).val() || '';
	const getArrayValue = id => $(id).val() || [];

	var diamondFilters = {
		Colors: getValue('#txtColorIds'),
		FromCarat: getValue('#minCarat'),
		ToCarat: getValue('#maxCarat'),
		Shapes: getArrayValue('#shapes'),
		Clarities: getValue('#txtClarityIds'),

		FromPrice: getValue('#minPrice'),
		ToPrice: getValue('#maxPrice'),
		Cuts: getValue('#txtCutIds'),
		FromRatio: getValue('#minLWRatio'),
		ToRatio: getValue('#maxLWRatio'),
		FromTable: getValue('#minTablePer'),
		ToTable: getValue('#maxTablePer'),
		FromDepth: getValue('#minDepthPer'),
		ToDepth: getValue('#maxDepthPer'),
		Polish: getValue('#txtPolishIds'),
		Fluor: getValue('#txtFluorIds'),
		Symmeties: getValue('#txtSymmetyIds')
	};

	function showLoader() {
		$('#txtLoader').show(); // This shows the loader
	}

	// Function to hide the loader
	function hideLoader() {
		$('#txtLoader').hide(); // This hides the loader
	}

	loadAllFunctions();

	$("#shape-list").on("click", ".shape-btn", function () {
		const shapeId = $(this).attr('shapeId');
		$(this).toggleClass('selected');

		if ($(this).hasClass('selected')) {
			if (!diamondFilters.Shapes.includes(shapeId)) {
				diamondFilters.Shapes.push(shapeId);
			}
		} else {
			const index = diamondFilters.Shapes.indexOf(shapeId);
			if (index !== -1) diamondFilters.Shapes.splice(index, 1);
		}

		getDiamondDataList(diamondFilters, page, rangeVal);
	});

});







});



async function GetShapeDesign() {
	const shapeListContainer = $('#filter-box #shape-list'); // Cache the DOM selector
	try {
		const response = await fetch('@SD.BaseApiUrl/api/diamondproperty/get-shape-list'); // Using native fetch
		const data = await response.json();

		if (Array.isArray(data) && data.length > 0) {
			const shapeButtons = data.map(shape => {
				return `
						 <div class="shape-btn text-center" shapeId='${shape.Id}'>
						<input type="hidden" value="${shape.Id}" />
						<img src="${shape.IconPath}" alt="${shape.Name}" style="height:60px;" />
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

		$("#carat-size-slider").slider({
			range: true,
			min: response.MinCaratSize,  // Set minimum from API response
			max: response.MaxCaratSize,  // Set maximum from API response
			step: 0.1, // Adjust step size as needed
			values: [response.MinCaratSize, response.MaxCaratSize / 2], // Default values
			slide: function (event, ui) {
				debugger;
				$("#minCarat").val(ui.values[0]);
				$("#maxCarat").val(ui.values[1]);

				diamondFilters.FromCarat = ui.values[0];
				diamondFilters.ToCarat = ui.values[1];

				getDiamondDataList(diamondFilters, page, rangeVal);
			}
		});

		// Set initial values
		$("#minCarat").val(response.MinCaratSize);
		$("#maxCarat").val(response.MaxCaratSize);

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

		// Store full color objects with Id and Name
		const colorList = response;

		// Ensure at least one color exists
		if (colorList.length === 0) {
			console.warn('No colors available');
			return;
		}

		// Step is 1 color per step
		const StepCount = 1;

		$("#color-slider").slider({
			range: true,
			min: 0,
			max: colorList.length - 1,
			step: StepCount,
			values: [0, colorList.length - 1],

			slide: function (event, ui) {
				const startIndex = ui.values[0];
				const endIndex = ui.values[1];

				const minColor = colorList[startIndex];
				const maxColor = colorList[endIndex];

				// Set color name fields
				$("#minColor").val(minColor.Name);
				$("#maxColor").val(maxColor.Name);


				// Get all Ids in selected range
				const selectedColorIds = colorList
					.slice(startIndex, endIndex + 1)
					.map(item => item.Id);

				// Store in hidden field
				$("#txtIds").val(selectedColorIds.join(','));

				// Set filter values
				diamondFilters.Colors = $("#txtColorIds").val();
				console.log(diamondFilters.Colors);

				// Call filter
				getDiamondDataList(diamondFilters, page, rangeVal);
			}
		});

		// Set initial values
		$("#minColor").val(colorList[0].Name);
		$("#maxColor").val(colorList[colorList.length - 1].Name);

		const initialColorIds = colorList.map(item => item.Id);
		$("#txtColorIds").val(initialColorIds.join(','));

	} catch (error) {
		console.error('Error fetching data: ', error);
		$('#filter-box #color-slider').html('<p>Failed to load colors</p>');
	}
}

async function GetCutData() {
	try {
		// Fetch cut list from the API
		const response = await $.ajax({
			url: '@(SD.BaseApiUrl)/api/diamondproperty/get-cut-list',
			method: 'GET',
			dataType: 'json'
		});

		// Validate response
		if (!Array.isArray(response) || response.length === 0) {
			console.warn('No cuts available');
			$('#filter-box #cut-slider').html('<p>No cuts available</p>');
			return;
		}

		const cutList = response;

		// Initialize the slider
		$("#cut-slider").slider({
			range: true,
			min: 0,
			max: cutList.length - 1,
			step: 1,
			values: [0, cutList.length - 1],
			slide: function (event, ui) {
				const startIndex = ui.values[0];
				const endIndex = ui.values[1];

				const minCut = cutList[startIndex];
				const maxCut = cutList[endIndex];

				// Display selected cut names
				$("#minCut").val(minCut.Name);
				$("#maxCut").val(maxCut.Name);

				// Get selected Cut IDs
				const selectedCutIds = cutList
					.slice(startIndex, endIndex + 1)
					.map(item => item.Id);

				// Store in hidden field
				$("#txtCutIds").val(selectedCutIds.join(','));

				// Update filter object
				diamondFilters.Cuts = $("#txtCutIds").val();
				console.log(diamondFilters.Cuts);

				// Call filter
				getDiamondDataList(diamondFilters, page, rangeVal);
			}
		});

		// Set initial displayed values
		$("#minCut").val(cutList[0].Name);
		$("#maxCut").val(cutList[cutList.length - 1].Name);

		// Set all cut IDs initially
		const initialCutIds = cutList.map(item => item.Id);
		$("#txtCutIds").val(initialCutIds.join(','));

	} catch (error) {
		console.error('Error fetching cut data:', error);
		$('#filter-box #cut-slider').html('<p>Failed to load cuts</p>');
	}
}

async function GetClarityData() {
	try {
		// Fetch clarity list from API
		const response = await $.ajax({
			url: '@(SD.BaseApiUrl)/api/diamondproperty/diamond-property/get-clarity-list',
			method: 'GET',
			dataType: 'json'
		});

		// Validate response
		if (!Array.isArray(response) || response.length === 0) {
			console.warn('No clarity available');
			$('#filter-box #Clarity-slider').html('<p>No clarity available</p>');
			return;
		}

		const clarityList = response;

		// Initialize the jQuery UI slider
		$("#Clarity-slider").slider({
			range: true,
			min: 0,
			max: clarityList.length - 1,
			step: 1,
			values: [0, clarityList.length - 1],
			slide: function (event, ui) {
				const fromIndex = ui.values[0];
				const toIndex = ui.values[1];

				const fromClarity = clarityList[fromIndex];
				const toClarity = clarityList[toIndex];

				// Display clarity names
				$("#minClarity").val(fromClarity.Name);
				$("#maxClarity").val(toClarity.Name);

				// Collect clarity IDs
				const selectedClarityIds = clarityList
					.slice(fromIndex, toIndex + 1)
					.map(item => item.Id);

				// Store in hidden field
				$("#txtClarityIds").val(selectedClarityIds.join(','));

				// Update filters
				diamondFilters.Clarities = $("#txtClarityIds").val();
				console.log(diamondFilters.Clarities);

				// Call filtering
				getDiamondDataList(diamondFilters, page, rangeVal);
			}
		});

		// Set initial display
		$("#minClarity").val(clarityList[0].Name);
		$("#maxClarity").val(clarityList[clarityList.length - 1].Name);

		// Set all clarity Ids initially
		const initialClarityIds = clarityList.map(item => item.Id);
		$("#txtClarityIds").val(initialClarityIds.join(','));

	} catch (error) {
		console.error('Error fetching clarity data: ', error);
		$('#filter-box #Clarity-slider').html('<p>Failed to load clarity</p>');
	}
}

async function GetPriceData() {
	try {
		// Fetching price data from the API
		const response = await $.ajax({
			url: '@(SD.BaseApiUrl)/api/diamondproperty/get-price-ranges', // API endpoint
			method: 'GET', // GET request
			dataType: 'json', // Expecting JSON response
		});

		// Ensure the response contains MaxPrice
		if (!response || typeof response.MaxPrice !== 'number') {
			throw new Error('Invalid response structure');
		}

		const maxPrice = response.MaxPrice;
		const minPrice = response.MinPrice;

		// Initialize the price slider
		$("#price-slider").slider({
			range: true,
			min: minPrice, // Minimum price is 0
			max: maxPrice, // Set maximum from API response
			step: 0.1, // Step size for the slider
			values: [minPrice, maxPrice], // Default values
			slide: function (event, ui) {
				debugger;
				// Update the min and max price input fields dynamically
				$("#minPrice").val(ui.values[0]);
				$("#maxPrice").val(ui.values[1]);

				diamondFilters = {
					FromPrice: $('#minPrice').val() ? $('#minPrice').val() : '',
					ToPrice: $('#maxPrice').val() ? $('#maxPrice').val() : ''
				};

				// diamondFilters.FromPrice=ui.values[0];
				// diamondFilters.ToPrice=ui.values[1];

				getDiamondDataList(diamondFilters, page, rangeVal);

			}
		});

		// Set initial values for the price input fields
		$("#minPrice").val(minPrice);
		$("#maxPrice").val(maxPrice);

	} catch (error) {
		// Handle any errors during the API request or slider setup
		console.error('Error fetching data: ', error);
		$('#filter-box #price-list').html('<p>Failed to load price ranges</p>');
	}
}

async function GetTableData() {
	try {
		// Fetch data from the API endpoint
		const response = await $.ajax({
			url: '@(SD.BaseApiUrl)/api/diamondproperty/get-table-ranges', // API endpoint
			method: 'GET', // GET request
			dataType: 'json', // Expecting JSON response
		});

		// Validate the response structure
		if (!response || typeof response.MaxValue !== 'number') {
			throw new Error('Invalid response structure');
		}

		const maxValue = response.MaxValue;

		// Initialize the slider only if it hasn't been initialized already
		const $slider = $("#TablePer-slider");
		if ($slider.data('initialized')) return; // Prevent re-initialization
		$slider.data('initialized', true); // Mark slider as initialized

		// Setup the slider with the max value
		$slider.slider({
			range: true,
			min: 0,
			max: maxValue,
			step: 0.1,
			values: [0, maxValue],
			slide: function (event, ui) {
				// Update the min and max values on slide
				$("#minTablePer").val(ui.values[0]);
				$("#maxTablePer").val(ui.values[1]);

				// Update filters and fetch the data list
				diamondFilters.FromTable = ui.values[0];
				diamondFilters.ToTable = ui.values[1];

				getDiamondDataList(diamondFilters, page, rangeVal);
			}
		});

		// Set initial values for the input fields
		$("#minTablePer").val(0);
		$("#maxTablePer").val(maxValue);

	} catch (error) {
		// Handle any errors during the API request or slider setup
		console.error('Error fetching data:', error);
		$('#filter-box #table-list').html('<p>Failed to load table ranges</p>');
	}
}

async function GetDepthData() {
	try {
		// Fetching price data from the API
		const response = await $.ajax({
			url: '@(SD.BaseApiUrl)/api/diamondproperty/get-depth-ranges', // API endpoint
			method: 'GET', // GET request
			dataType: 'json', // Expecting JSON response
		});

		// Ensure the response contains MaxPrice
		if (!response || typeof response.MaxValue !== 'number') {
			throw new Error('Invalid response structure');
		}

		const maxValue = response.MaxValue;

		// Initialize the price slider
		$("#DepthPer-slider").slider({
			range: true,
			min: 0, // Minimum price is 0
			max: maxValue, // Set maximum from API response
			step: 0.1, // Step size for the slider
			values: [0, maxValue], // Default values
			slide: function (event, ui) {
				// Update the min and max price input fields dynamically
				$("#minDepthPer").val(ui.values[0]);
				$("#maxDepthPer").val(ui.values[1]);

				diamondFilters.FromTable = $("#minDepthPer").val();
				diamondFilters.ToTable = $("#maxDepthPer").val();

				getDiamondDataList(diamondFilters, page, rangeVal);
			}
		});

		// Set initial values for the price input fields
		$("#minDepthPer").val(0);
		$("#maxDepthPer").val(maxValue);

	} catch (error) {
		// Handle any errors during the API request or slider setup
		console.error('Error fetching data: ', error);
		$('#filter-box #DepthPer-list').html('<p>Failed to load depth ranges</p>');
	}
}

async function GetRatioData() {
	try {
		// Fetching data from API
		var response = await $.ajax({
			url: '@(SD.BaseApiUrl)/api/diamondproperty/get-ratio-list',
			method: 'GET',
			dataType: 'json',
		});

		// Extract color names
		const ratioList = response.map(item => item.Name);

		// Ensure that there's at least one color to avoid errors
		if (ratioList.length === 0) {
			console.warn('Not found ratio.');
			return;
		}

		// Calculate slider step as the index difference
		const StepCount = 1;

		$("#LWRatio-slider").slider({
			range: true,
			min: 0,
			max: ratioList.length - 1,
			step: StepCount,
			values: [0, ratioList.length - 1],
			slide: function (event, ui) {
				// Set the selected color values based on slider indices
				$("#minLWRatio").val(ratioList[ui.values[0]]);
				$("#maxLWRatio").val(ratioList[ui.values[1]]);

				diamondFilters.FromTable = $("#minLWRatio").val();
				diamondFilters.ToTable = $("#maxLWRatio").val();

				getDiamondDataList(diamondFilters, page, rangeVal);
			}
		});

		// Set initial values for the min and max color fields
		$("#minLWRatio").val(ratioList[0]);
		$("#maxLWRatio").val(ratioList[ratioList.length - 1]);

	} catch (error) {
		console.error('Error fetching data: ', error);
		$('#filter-box #LWRatio-slider').html('<p>Failed to load Ratio</p>');
	}
}

async function GetPolishData() {
	try {
		// Fetching data from API
		const response = await $.ajax({
			url: '@(SD.BaseApiUrl)/api/diamondproperty/get-polish-list',
			method: 'GET',
			dataType: 'json'
		});

		// Check for valid response
		if (!Array.isArray(response) || response.length === 0) {
			console.warn('No polish data found.');
			$('#filter-box #Polish-slider').html('<p>No polish available</p>');
			return;
		}

		const polishList = response;

		$("#Polish-slider").slider({
			range: true,
			min: 0,
			max: polishList.length - 1,
			step: 1,
			values: [0, polishList.length - 1],
			slide: function (event, ui) {
				const fromIndex = ui.values[0];
				const toIndex = ui.values[1];

				const fromPolish = polishList[fromIndex];
				const toPolish = polishList[toIndex];

				$("#minPolish").val(fromPolish.Name);
				$("#maxPolish").val(toPolish.Name);

				const selectedPolishIds = polishList
					.slice(fromIndex, toIndex + 1)
					.map(item => item.Id);

				$("#txtPolishIds").val(selectedPolishIds.join(','));
				diamondFilters.Polishes = $("#txtPolishIds").val();

				getDiamondDataList(diamondFilters, page, rangeVal);
			}
		});

		// Set initial values
		$("#minPolish").val(polishList[0].Name);
		$("#maxPolish").val(polishList[polishList.length - 1].Name);

		const initialPolishIds = polishList.map(item => item.Id);
		$("#txtPolishIds").val(initialPolishIds.join(','));
		diamondFilters.Polishes = $("#txtPolishIds").val();

		// Initial load
		getDiamondDataList(diamondFilters, page, rangeVal);

	} catch (error) {
		console.error('Error fetching polish data: ', error);
		$('#filter-box #Polish-slider').html('<p>Failed to load Polish</p>');
	}
}

async function GetFluorData() {
	try {
		// Fetching data from API
		const response = await $.ajax({
			url: '@(SD.BaseApiUrl)/api/diamondproperty/get-fluor-list',
			method: 'GET',
			dataType: 'json'
		});

		// Validate response
		if (!Array.isArray(response) || response.length === 0) {
			console.warn('No fluor data found.');
			$('#filter-box #Fluor-slider').html('<p>No fluor available</p>');
			return;
		}

		const fluorList = response;

		$("#Fluor-slider").slider({
			range: true,
			min: 0,
			max: fluorList.length - 1,
			step: 1,
			values: [0, fluorList.length - 1],
			slide: function (event, ui) {
				const fromIndex = ui.values[0];
				const toIndex = ui.values[1];

				const fromFluor = fluorList[fromIndex];
				const toFluor = fluorList[toIndex];

				$("#minFluor").val(fromFluor.Name);
				$("#maxFluor").val(toFluor.Name);

				const selectedFluorIds = fluorList
					.slice(fromIndex, toIndex + 1)
					.map(item => item.Id);

				$("#txtFluorIds").val(selectedFluorIds.join(','));
				diamondFilters.Fluorescence = $("#txtFluorIds").val();

				getDiamondDataList(diamondFilters, page, rangeVal);
			}
		});

		// Set initial values
		$("#minFluor").val(fluorList[0].Name);
		$("#maxFluor").val(fluorList[fluorList.length - 1].Name);

		const initialFluorIds = fluorList.map(item => item.Id);
		$("#txtFluorIds").val(initialFluorIds.join(','));
		diamondFilters.Fluorescence = $("#txtFluorIds").val();

		getDiamondDataList(diamondFilters, page, rangeVal);

	} catch (error) {
		console.error('Error fetching fluor data: ', error);
		$('#filter-box #Fluor-slider').html('<p>Failed to load fluor</p>');
	}
}

async function GetSymmetryData() {
	try {
		// Fetching data from API
		const response = await $.ajax({
			url: '@(SD.BaseApiUrl)/api/diamondproperty/get-symmetry-list',
			method: 'GET',
			dataType: 'json',
		});

		// Validate response
		if (!Array.isArray(response) || response.length === 0) {
			console.warn('No symmetry data found.');
			$('#filter-box #Symmetry-slider').html('<p>No symmetry available</p>');
			return;
		}

		const symmetryList = response;

		$("#Symmetry-slider").slider({
			range: true,
			min: 0,
			max: symmetryList.length - 1,
			step: 1,
			values: [0, symmetryList.length - 1],
			slide: function (event, ui) {
				const fromIndex = ui.values[0];
				const toIndex = ui.values[1];

				const fromSym = symmetryList[fromIndex];
				const toSym = symmetryList[toIndex];

				$("#minSymmetry").val(fromSym.Name);
				$("#maxSymmetry").val(toSym.Name);

				const selectedSymmetryIds = symmetryList
					.slice(fromIndex, toIndex + 1)
					.map(item => item.Id);

				$("#txtSymmetyIds").val(selectedSymmetryIds.join(','));
				diamondFilters.Symmetry = $("#txtSymmetyIds").val();

				getDiamondDataList(diamondFilters, page, rangeVal);
			}
		});

		// Set initial values for the min and max symmetry fields
		$("#minSymmetry").val(symmetryList[0].Name);
		$("#maxSymmetry").val(symmetryList[symmetryList.length - 1].Name);

		const initialSymmetryIds = symmetryList.map(item => item.Id);
		$("#txtSymmetyIds").val(initialSymmetryIds.join(','));
		diamondFilters.Symmetry = $("#txtSymmetyIds").val();

		getDiamondDataList(diamondFilters, page, rangeVal);

	} catch (error) {
		console.error('Error fetching symmetry data: ', error);
		$('#filter-box #Symmetry-slider').html('<p>Failed to load symmetry</p>');
	}
}

async function getDiamondDataList(diamondFilters, pageNumber = 1, pageSize = 10) {
	return $.ajax({
		//url: '@Url.Action("GetDiamondList", "Diamond")', // Change this to your actual route
		url: '/Diamond/GetDiamondList', // Change this to your actual route
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

async function ResetAllData() {
	GetShapeDesign();
	GetCarat();
	GetCutData();
	GetColorData();
	GetClarityData();
	GetPriceData();
	GetPolishData();
	GetFluorData();
	GetSymmetryData();
	GetTableData();
	GetDepthData();
	GetRatioData();
	diamondFilters = [];
	getDiamondDataList(diamondFilters, page, rangeVal);
}

async function loadAllFunctions() {
	showLoader();
	try {
		await Promise.all([
			GetShapeDesign(),
			GetCarat(),
			GetCutData(),
			GetColorData(),
			GetClarityData(),
			GetPriceData(),
			GetPolishData(),
			GetFluorData(),
			GetSymmetryData(),
			GetTableData(),
			GetDepthData(),
			GetRatioData(),
		]);
		await getDiamondDataList(diamondFilters, page, rangeVal);
	} catch (error) {
		console.error("An error occurred while loading data:", error);
	}
	hideLoader();
}

