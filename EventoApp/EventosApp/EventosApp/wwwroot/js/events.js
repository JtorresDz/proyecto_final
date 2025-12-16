fetch('/api/events/list')
    .then(r => r.json())
    .then(events => {
        eventList.innerHTML = events.map(e =>
            `<div><strong>${e.title}</strong> - ${e.startDate}</div>`
        ).join('');
    });


if (document.getElementById('eventForm')) {
    eventForm.onsubmit = async e => {
        e.preventDefault();
        await fetch('/api/events/create', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                title: title.value,
                description: description.value,
                startDate: startDate.value
            })
        });
        alert('Evento creado');
    };
}