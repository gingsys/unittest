function callMSGraph(endpoint, token, callback) {
    const headers = new Headers();
    const bearer = `Bearer ${token}`;

    headers.append("Authorization", bearer);

    const options = {
        method: "GET",
        headers: headers
    };

    console.log('request made to Graph API EXTRA INFO at: ' + new Date().toString());

    fetch(endpoint, options)
        .then(response => response.json())
        .then(response => localStorage.setItem("EmpresaAD", response.companyName),
            localStorage.setItem("NumeroDocumento", response.postalCode))
        .catch(error => console.log(error))
}