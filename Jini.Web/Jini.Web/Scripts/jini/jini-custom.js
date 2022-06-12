function adjustWidthForMainPage() {
	if ( $( ".content" ).hasClass( "main-page" ) ) {
		$( ".content" ).css( "max-width", "100%" );
		$( ".container" ).css( { "max-width":"none", "width":"98%" } );
		$( ".header-content-main" ).css( { "max-width":"none", "width":"98%" } );
	}
}

function adjustBreadcrumbs() {
	var sidebar = $( "section.sidebar" );
	
	if ( sidebar.length ) {
		$( ".header-content-main" ).css( "max-width", "1290px" );
		$( ".breadcrumbs > ul" ).css( "max-width", "1290px" );
	}
}

function adjustPriceInfoText() {
	var priceInfo = $( ".price-info" );
	
	if ( !priceInfo.length ) return;
	
	priceInfo.expander({
		slicePoint:       250,
		expandPrefix:     "…",
		expandText:       "Vis mere",
		userCollapseText: "Vis mindre",
		expandEffect:     "slideDown",
		expandSpeed:      500,
		collapseEffect:   "slideUp",
		collapseSpeed:    500
	});
}

function toggleLicenseSelectionInPreview() {
	
}

$(document).ready(function() {
	adjustPriceInfoText();
	toggleLicenseSelectionInPreview();
});
