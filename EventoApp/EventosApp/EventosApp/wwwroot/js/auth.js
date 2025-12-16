async function login() {
  const body = { username: username.value, password: password.value };

  const res = await fetch('/api/account/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  });

  if (!res.ok) {
    alert('Credenciales incorrectas');
    return;
  }

  const data = await res.json();
  sessionStorage.setItem('username', data.username);
  sessionStorage.setItem('isAdmin', data.isAdmin);
  location = 'home.html';
}
