import { useCallback, useEffect, useRef, useState } from 'react';
import { autocomplete, search } from '../api/userApi';
import type { User } from '../types/user';

export function useSearch() {
  const [query, setQuery] = useState('');
  const [suggestions, setSuggestions] = useState<string[]>([]);
  const [results, setResults] = useState<User[]>([]);
  const [hasSearched, setHasSearched] = useState(false);
  const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const skipAutocompleteRef = useRef(false);

  useEffect(() => {
    if (query.length < 2) {
      setSuggestions([]);
      return;
    }
    if (skipAutocompleteRef.current) {
      skipAutocompleteRef.current = false;
      return;
    }
    if (debounceRef.current) clearTimeout(debounceRef.current);
    debounceRef.current = setTimeout(async () => {
      const data = await autocomplete(query);
      setSuggestions(data);
    }, 250);
    return () => { if (debounceRef.current) clearTimeout(debounceRef.current); };
  }, [query]);

  const runSearch = useCallback(async (q: string) => {
    setSuggestions([]);
    setHasSearched(true);
    const data = await search(q);
    setResults(data);
  }, []);

  const selectSuggestion = useCallback((name: string) => {
    skipAutocompleteRef.current = true;
    setQuery(name);
    runSearch(name);
  }, [runSearch]);


  return { query, setQuery, suggestions, results, hasSearched, runSearch, selectSuggestion };
}
