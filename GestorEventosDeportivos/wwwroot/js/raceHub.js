// raceHub.js - cliente SignalR en el navegador para actualizaciones de carrera

async function loadScript(src) {
  return new Promise((resolve, reject) => {
    const s = document.createElement('script');
    s.src = src;
    s.async = true;
    s.defer = true;
    s.onload = resolve;
    s.onerror = reject;
    document.head.appendChild(s);
  });
}

export async function setupRaceHub(dotNetRef, raceId) {
  if (!window.signalR) {
    try {
      await loadScript('/lib/signalr/signalr.min.js');
    } catch (e1) {
      try {
        await loadScript('/lib/signalr/signalr.js');
      } catch (e2) {
        console.warn('No se pudo cargar la librería de SignalR desde /lib. Verifique que exista /wwwroot/lib/signalr/signalr.min.js');
        throw e2;
      }
    }
  }

  if (window.__raceHubConn) {
    try { await window.__raceHubConn.stop(); } catch {}
    window.__raceHubConn = null;
  }

  const connection = new window.signalR.HubConnectionBuilder()
    .withUrl('/hubs/race')
    .withAutomaticReconnect()
    .build();

  connection.on('RaceUpdated', (rid) => {
    if (rid === raceId) {
      try {
        dotNetRef.invokeMethodAsync('OnRaceUpdatedDotNet', rid);
      } catch {}
    }
  });

  await connection.start();
  await connection.invoke('JoinRaceGroup', raceId);
  window.__raceHubConn = connection;
}

export async function stopRaceHub() {
  if (window.__raceHubConn) {
    try { await window.__raceHubConn.stop(); } catch {}
    window.__raceHubConn = null;
  }
}
