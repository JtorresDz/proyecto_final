const user = sessionStorage.getItem('username');
const isAdmin = sessionStorage.getItem('isAdmin') === 'true';

const myEventsCheckboxGroup = document.querySelector('.checkbox-group');

if (isAdmin && myEventsCheckboxGroup) {
    myEventsCheckboxGroup.style.display = 'none';
}


if (!user) location.href = 'login.html';

document.getElementById('userInfo').innerText = `Hola, ${user}`;

const adminBtn = document.getElementById('adminBtn');
if (isAdmin) {
    adminBtn.style.display = 'inline-block';
    adminBtn.addEventListener('click', () => {
        location.href = 'admin-panel.html';
    });
}

document.getElementById('logoutBtn').addEventListener('click', logout);

async function logout() {
    try {
        await fetch('/api/account/logout', {
            method: 'POST',
            credentials: 'include'
        });
    } catch { }

    sessionStorage.clear();
    location.href = 'index.html';
}

let allEvents = [];

fetch('/api/events/list')
    .then(r => r.json())
    .then(events => {
        allEvents = events;
        renderEvents(events);
    });

function renderEvents(events) {
    const list = document.getElementById('list');
    list.innerHTML = '';

    if (!events.length) {
        list.innerHTML = '<p>No hay eventos para mostrar.</p>';
        return;
    }

    events.forEach(e => {
        const div = document.createElement('div');
        div.className = 'event';

        div.innerHTML = `
            <h3>${e.title}</h3>
            <p>${e.description ?? ''}</p>
            <p><strong>Tipo:</strong> ${e.tipo}</p>
            <p><strong>Localización:</strong> ${e.localizacion}</p>
            <p><strong>Inicio:</strong> ${new Date(e.startDate).toLocaleString()}</p>
            <p><strong>Final:</strong> ${e.finalDate ? new Date(e.finalDate).toLocaleString() : 'No definido'}</p>
            <p><strong>Inscritos:</strong> ${e.inscritos}</p>
            <div id="actions-${e.id}"></div>
        `;

        list.appendChild(div);
        checkRegistration(e.id);
    });
}

function checkRegistration(eventId) {
    const container = document.getElementById(`actions-${eventId}`);

    if (isAdmin) {
        container.innerHTML = `<span class="badge admin">Administrador</span>`;
        return;
    }

    fetch(`/api/events/is-registered/${eventId}`)
        .then(r => r.ok ? r.json() : null)
        .then(data => {
            container.innerHTML = '';

            if (!data || !data.isRegistered) {
                const btn = document.createElement('button');
                btn.className = 'btn-primary';
                btn.textContent = 'Registrarse';
                btn.addEventListener('click', () => register(eventId));
                container.appendChild(btn);
                return;
            }

            container.innerHTML = `<span class="badge success">Ya inscrito</span>`;

            const btn = document.createElement('button');
            btn.className = 'btn-danger';
            btn.textContent = 'Cancelar inscripción';
            btn.addEventListener('click', () => cancel(eventId));
            container.appendChild(btn);
        });
}

function register(eventId) {
    fetch('/api/events/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ EventoId: eventId })
    })
        .then(r => r.ok ? location.reload() : r.text().then(t => alert(t)));
}

function cancel(eventId) {
    fetch('/api/events/cancel', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ EventoId: eventId })
    })
        .then(r => r.ok ? location.reload() : alert('Error al cancelar inscripción'));
}

document.getElementById('btnFiltrar')?.addEventListener('click', applyFilters);
document.getElementById('btnLimpiar')?.addEventListener('click', clearFilters);

document.getElementById('searchTitle')?.addEventListener('keydown', e => {
    if (e.key === 'Enter') {
        e.preventDefault();
        applyFilters();
    }
});

async function applyFilters() {
    const title = document.getElementById('searchTitle')?.value.toLowerCase() || '';
    const type = document.getElementById('filterType')?.value.toLowerCase() || '';
    const from = document.getElementById('dateFrom')?.value || '';
    const to = document.getElementById('dateTo')?.value || '';
    const onlyMine = document.getElementById('onlyMyEvents')?.checked || false;

    let filtered = [...allEvents];

    if (title) filtered = filtered.filter(e => e.title.toLowerCase().includes(title));
    if (type) filtered = filtered.filter(e => e.tipo.toLowerCase().includes(type));
    if (from) filtered = filtered.filter(e => new Date(e.startDate) >= new Date(from));
    if (to) filtered = filtered.filter(e => !e.finalDate || new Date(e.finalDate) <= new Date(to));

    if (onlyMine && !isAdmin) {
        const mine = [];
        for (const ev of filtered) {
            const r = await fetch(`/api/events/is-registered/${ev.id}`);
            if (!r.ok) continue;
            const data = await r.json();
            if (data.isRegistered) mine.push(ev);
        }
        filtered = mine;
    }

    renderEvents(filtered);
}

function clearFilters() {
    document.getElementById('searchTitle') && (searchTitle.value = '');
    document.getElementById('filterType') && (filterType.value = '');
    document.getElementById('dateFrom') && (dateFrom.value = '');
    document.getElementById('dateTo') && (dateTo.value = '');
    document.getElementById('onlyMyEvents') && (onlyMyEvents.checked = false);

    renderEvents(allEvents);
}
