import React from 'react'
import ReactDOM from 'react-dom'
import './index.css'
import App from './App'
import * as serviceWorker from './serviceWorker'
import axios from 'axios'

axios.interceptors.response.use(function (response) {
  return response
}, function (error) {
  if (error && error.response && error.response.status === 401) {
    localStorage.removeItem('userData')
    window.location.replace('/')
  } else {
    return Promise.reject(error)
  }
})
ReactDOM.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
  document.getElementById('root')
)

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister()
