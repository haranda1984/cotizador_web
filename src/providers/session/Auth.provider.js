import React, { useReducer, useContext } from 'react'
import { AuthReducer } from './Auth.reducer'

const initialState = {
  loading: false,
  loggedIn: false,
  user: {}
}

const AuthContext = React.createContext()
const useAuth = () => {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('NO CONTEXT')
  }
  return context
}

const useAuthUser = () => {
  const context = useAuth()
  if (!context.isAuthenticated) {
    throw new Error('NO AUTH')
  }
  return context.user
}
// eslint-disable-next-line react/prop-types
const AuthProvider = ({ children }) => {
  const [state, dispatch] = useReducer(AuthReducer, initialState)
  return (<AuthContext.Provider value={{ state, dispatch }}>
    {children}
  </AuthContext.Provider>)
}

export default AuthProvider

export { initialState, AuthContext, useAuthUser, useAuth }
