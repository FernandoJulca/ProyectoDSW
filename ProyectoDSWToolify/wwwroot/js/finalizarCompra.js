document.querySelectorAll('input[name="MetodoPago"]').forEach(radio => {
	radio.addEventListener('change', function () {
		const datosTarjetaDiv = document.getElementById('datosTarjeta');
		if (this.value === 'R') {
			datosTarjetaDiv.style.display = 'block';
		} else {
			datosTarjetaDiv.style.display = 'none';
		}
	});
});


// Formatear número de tarjeta (simple)
function formatearNumeroTarjeta(input) {
	let value = input.value.replace(/\D/g, '').substring(0, 16);
	let formatted = '';
	for (let i = 0; i < value.length; i += 4) {
		if (i > 0) formatted += ' ';
		formatted += value.substring(i, i + 4);
	}
	input.value = formatted;
}

// Solo números para inputs
function soloNumeros(event) {
	let charCode = (event.which) ? event.which : event.keyCode;
	if (charCode > 31 && (charCode < 48 || charCode > 57)) {
		event.preventDefault();
		return false;
	}
	return true;
}
document.addEventListener("DOMContentLoaded", function () {
	const radioPresencial = document.getElementById("pagoPresencial");
	const radioDelivery = document.getElementById("pagoDelivery");
	const tarjetaDiv = document.getElementById("datosTarjeta");
	const tarjetaInputs = tarjetaDiv.querySelectorAll("input, select");

	function toggleTarjetaFields() {
		if (radioDelivery.checked) {
			tarjetaDiv.style.display = "block";
			tarjetaInputs.forEach(el => el.required = true);
		} else {
			tarjetaDiv.style.display = "none";
			tarjetaInputs.forEach(el => el.required = false);
		}
	}

	radioPresencial.addEventListener("change", toggleTarjetaFields);
	radioDelivery.addEventListener("change", toggleTarjetaFields);

	toggleTarjetaFields();
});