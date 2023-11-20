
function initializeVeriff(sessionToken, apiKey) {
    const veriff = Veriff({
        host: 'https://stationapi.veriff.com',
        apiKey: apiKey,
        parentId: 'veriff-root',
        onSession: function(err, response) {
            if (!err && response) {
                window.veriffSDK.createVeriffFrame({ url: response.verification.url });
            } else {
                console.error('Error initializing Veriff:', err);
            }
        }
    });

    veriff.mount({
        formLabel: {
            vendorData: 'Order number'
        }
    });
}