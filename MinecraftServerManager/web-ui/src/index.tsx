import React, { useEffect } from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import 'bootstrap/dist/css/bootstrap.css';
import App from './App';
import reportWebVitals from './reportWebVitals';
import { BrowserRouter, Route, Routes, useNavigate } from 'react-router-dom';

const root = ReactDOM.createRoot(
	document.getElementById('root') as HTMLElement
);
root.render(
	<React.StrictMode>
		<BrowserRouter>
			<Routes>
				<Route path="/servers" element={<App />}>
					<Route path=":serverId" element={<App />} />
					<Route path="*" element={<p>There's nothing here!</p>} />
				</Route>

				<Route path="*" element={<Redirect destination="/servers/ala" />} />
			</Routes>
		</BrowserRouter>
	</React.StrictMode>
);

interface RedirectProps {
	destination: string
}
function Redirect(props: RedirectProps) {
	console.info(`Redirecting to ${props.destination}`)
	const navigate = useNavigate();
	useEffect(() => {
		navigate(props.destination)
	});
	return null
}

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals(); 
